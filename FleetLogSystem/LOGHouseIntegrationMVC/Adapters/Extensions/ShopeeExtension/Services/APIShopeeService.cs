using LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto;
using LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Request;
using LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Response;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using LOGHouseSystem.Adapters.Extensions.BaseOAUTH2Extension;
using LOGHouseSystem.Adapters.BaseOAUTH2Extension.Dto;
using System.Text;
using System.Security.Cryptography;
using System.Net.Http.Headers;

namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension
{
    public abstract class APIShopeeService : BaseOAUTH2BaseService, IAPIShopeeService
    {
        public string HOST = "https://partner.shopeemobile.com";
        public string REDIRECT_URL = Environment.ShopeeEnvironment.RedirectUrl;

        private readonly IDataShopeeService _dataShopeeService;        

        private readonly RestClient _client;
        private List<ShopDataShopeeDto> _shopDateShopeeDto = new List<ShopDataShopeeDto>();        

        public APIShopeeService(IDataShopeeService dataShopeeService) : base(dataShopeeService)
        {
            _dataShopeeService = dataShopeeService;

            REDIRECT_URL = GetAuthorizationRedirectUrl();
            HOST = GetHostUrl();
            REFRESH_TOKEN_TIME = 30 * 24 * 60 * 60; // days * hours * minutes * seconds
            PartnerName = "Shopee";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        /// <exception cref="ShopDataNotFoundException"></exception>
        private async Task<ShopDataShopeeDto> GetShopeeDataByClinetId(int clientId)
        {
            var data = await _dataShopeeService.GetShopData(clientId);

            // if isn't founded, trigger an exception
            if (data == null)
            {
                throw new AuthenticationNotFoundException(string.Format("Dados do parceiro da {0} não estão configurados.", PartnerName));
            }

            return data;
        }

        /// <summary>
        /// Take the access token by refresh token
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override async Task<AuthenticationDto> TakeAccessTokenByRefreshToken(string refreshToken, int clientId)
        {
            var requestDto = await GetAccessTokenByRefreshToken(refreshToken, clientId);
            return ConvertResponseToDto(requestDto, clientId);
        }

        #region OAUTH2
        public async override Task<string> GenerateAuthorizationCodeUrl(int clientId)
        {
            var host = HOST;
            var path = "/api/v2/shop/auth_partner";
            var redirectUrl = REDIRECT_URL.Replace("[clientId]", clientId.ToString());

            var timest = DateTimeOffset.Now.ToUnixTimeSeconds();
            var baseString = $"{Environment.ShopeeEnvironment.PartnerId}{path}{timest}";
            var sign = ComputeHMACSHA256Hash(baseString, Environment.ShopeeEnvironment.PartnerKey);
            var url = $"{host}{path}?partner_id={Environment.ShopeeEnvironment.PartnerKey}&timestamp={timest}&sign={sign}&redirect={redirectUrl}";

            return url;
        }

        public string GenerateAuthorizationCodeUrlSave(int clientId)
        {
            var host = HOST;
            var path = "/api/v2/shop/auth_partner";
            var redirectUrl = REDIRECT_URL.Replace("[clientId]", clientId.ToString());

            var timest = DateTimeOffset.Now.ToUnixTimeSeconds();
            var baseString = $"{Convert.ToInt32(Environment.ShopeeEnvironment.PartnerId)}{path}{timest}";
            var sign = ComputeHMACSHA256Hash(baseString, Environment.ShopeeEnvironment.PartnerKey);
            var url = $"{host}{path}?partner_id={Convert.ToInt32(Environment.ShopeeEnvironment.PartnerId)}&timestamp={timest}&sign={sign}&redirect={redirectUrl}";

            return url;
        }

        public async override Task<AuthenticationAPIResponseDto> GetAccessTokenByCode(string code, int clientId)
        {
            return await GetAccessTokenByCodeByShopId(code, clientId);            
        }

        public async Task<AuthenticationAPIResponseDto> GetAccessTokenByCodeByAccountId(string code, int clientId)
        {
            var data = await GetShopeeDataByClinetId(clientId);
            string path = "/api/v2/auth/token/get";

            var body = new GetTokenAccountLevelShopeeAccountRequestDto()
            {
                Code = code,
                PartnerId = Convert.ToInt32(Environment.ShopeeEnvironment.PartnerId),
                MainAccountId = Convert.ToInt32(data.MainAccountId)
            };

            var bodyString = JsonConvert.SerializeObject(body);

            var response = await PostAuthentication(path, bodyString, clientId);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Não foi possível realizar a autenticação na Shopee. " + response.Content);
            }

            AuthenticationAPIResponseDto responseContent = JsonConvert.DeserializeObject<AuthenticationAPIResponseDto>(response.Content);

            return responseContent;
        }



        public async Task<AuthenticationAPIResponseDto> GetAccessTokenByCodeByShopId(string code, int clientId)
        {
            var data = await GetShopeeDataByClinetId(clientId);

            string path = "/api/v2/auth/token/get";

            var body = new GetTokenAccountLevelShopeeRequestDto()
            {
                Code = code,                
                PartnerId = Convert.ToInt32(Environment.ShopeeEnvironment.PartnerId),
                ShopId = Convert.ToInt32(data.ShopId)
            };

            var bodyString = JsonConvert.SerializeObject(body);

            var response = await PostAuthentication(path, bodyString, clientId);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Não foi possível realizar a autenticação na Shopee. " + response.Content);
            }

            AuthenticationAPIResponseDto responseContent = JsonConvert.DeserializeObject<AuthenticationAPIResponseDto>(response.Content);

            return responseContent;
        }

        public async Task<AuthenticationAPIResponseDto> GetAccessTokenByRefreshToken(string refreshToken, int clientId)
        {
            var data = await GetShopeeDataByClinetId(clientId);

            var path = "/api/v2/auth/access_token/get";

            var body = new
            {
                partner_id = Convert.ToInt32(Environment.ShopeeEnvironment.PartnerId),
                refresh_token = refreshToken,
                shop_id = Convert.ToInt32(data.ShopId)
            };

            var bodyContent = JsonConvert.SerializeObject(body);

            var response = await PostAuthentication(path, bodyContent, clientId);
            var content = response.Content;

            AuthenticationAPIResponseDto result = JsonConvert.DeserializeObject<AuthenticationAPIResponseDto>(content);

            return result;
        }

        public string ComputeHMACSHA256Hash(string message, string key)
        {
            byte[] baseStringBytes = Encoding.UTF8.GetBytes(message);
            byte[] partnerKeyBytes = Encoding.UTF8.GetBytes(key);

            var sha256 = new HMACSHA256(partnerKeyBytes);
            byte[] signBytes = sha256.ComputeHash(baseStringBytes);
            string sign = BitConverter.ToString(signBytes).Replace("-", "").ToLower();
            return sign;
        }

        private async Task<HttpResponseMessage> Post(string path, string body, int clientId)
        {
            var accessToken = await GetAccessToken(clientId);

            var data = await GetShopeeDataByClinetId(clientId);

            var client = new HttpClient();            

            var timestamp = GetTimeStamp().ToString();
            string tmpBaseString = string.Format("{0}{1}{2}{3}{4}", Convert.ToInt32(Environment.ShopeeEnvironment.PartnerId), path, timestamp, accessToken.AccessToken, data.ShopId);
            string sign = ComputeHMACSHA256Hash(tmpBaseString, Environment.ShopeeEnvironment.PartnerKey);
            
            // Generating URL
            var url = string.Format("{0}{1}?partner_id={2}&timestamp={3}&sign={4}&shop_id={5}&access_token={6}", HOST, path, Environment.ShopeeEnvironment.PartnerId, timestamp, sign, data.ShopId, accessToken.AccessToken);
            
            client.DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header            

            var content = new StringContent(body,
                                    Encoding.UTF8,
                                    "application/json");

            Log.Info(string.Format("POST URL {0} - Body {1} - Header - Content-Type: application/json", url, body));

            var response = await client.PostAsync(url, content);

            return response;
        }

        private async Task<HttpResponseMessage> Get(string path, string parameters, int clientId)
        {
            var accessToken = await GetAccessToken(clientId);

            var data = await GetShopeeDataByClinetId(clientId);

            var client = new HttpClient();

            var timestamp = GetTimeStamp().ToString();
            string tmpBaseString = string.Format("{0}{1}{2}{3}{4}", Convert.ToInt32(Environment.ShopeeEnvironment.PartnerId), path, timestamp, accessToken.AccessToken, data.ShopId);
            string sign = ComputeHMACSHA256Hash(tmpBaseString, Environment.ShopeeEnvironment.PartnerKey);

            // Generating URL
            var url = string.Format("{0}{1}?{2}&partner_id={3}&timestamp={4}&sign={5}&shop_id={6}&access_token={7}", HOST, path, parameters, Environment.ShopeeEnvironment.PartnerId, timestamp, sign, data.ShopId, accessToken.AccessToken);

            Log.Info(string.Format("GET URL {0}", url));

            var response = await client.GetAsync(url);

            return response;
        }

        private async Task<RestResponse> PostAuthentication(string path, string body, int clientId)
        {
            var data = await GetShopeeDataByClinetId(clientId);
            
            var timest = GetTimeStamp();            

            var baseString = string.Format("{0}{1}{2}", Convert.ToInt32(Environment.ShopeeEnvironment.PartnerId), path, timest);
            string sign = ComputeHMACSHA256Hash(baseString, Environment.ShopeeEnvironment.PartnerKey);

            var url = string.Format("{0}{1}?partner_id={2}&timestamp={3}&sign={4}", HOST, path, Convert.ToInt32(Environment.ShopeeEnvironment.PartnerId), timest, sign);           

            var client = new RestClient();
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddBody(body);

            var response = await client.ExecuteAsync(request);
            return response;
        }

        #endregion

        public async Task<HttpResponseMessage> DownloadShippingDocumentAsync(List<ShipmentListShiptShopeeDto> orderList, string documentType, int clientId, bool secondTrying = false)
        {
            var path = $"/api/v2/logistics/download_shipping_document";           

            var dataContent = new DownloadShippingDocumentShopeeRequestDto();
            dataContent.ShippingDocumentType = documentType;
            dataContent.OrderList = orderList;

            var strContent = JsonConvert.SerializeObject(dataContent);

            var response = await Post(path, strContent, clientId);

            if (!(await ThereAreErrorRequest(response, clientId, secondTrying)))
            {
                return response;
            }
            else
            {
                await DownloadShippingDocumentAsync(orderList, documentType, clientId, true);
            }

            return null;
        }

        public async Task<HttpResponseMessage> GetShippingDetails(string orderSn, int clientId, bool secondTrying = false)
        {
            var path = $"/api/v2/order/get_order_detail";

            var parameters = string.Format("order_sn_list={0}", orderSn);

            var response = await Get(path, parameters, clientId);

            if (!(await ThereAreErrorRequest(response, clientId, secondTrying)))
            {
                return response;
            }
            else
            {
                await GetShippingDetails(orderSn, clientId, true);
            }

            return null;
        }


        public async Task<HttpResponseMessage> GeneratingShippingDocumentAsync(CreateShippingDocumentShopeeRequestDto orderList, int clientId, bool secondTrying = false)
        {
            var path = $"/api/v2/logistics/create_shipping_document";

            var strContent = JsonConvert.SerializeObject(orderList);

            var response = await Post(path, strContent, clientId);

            if (!(await ThereAreErrorRequest(response, clientId, secondTrying)))
            {
                return response;
            }
            else
            {
                await GeneratingShippingDocumentAsync(orderList, clientId, true);
            }

            return null;
        }

        public async Task<HttpResponseMessage> ShipOrderAsync(ShipOrderShopeeRequest data, int clientId, bool secondTrying = false)
        {
            var path = $"/api/v2/logistics/ship_order";

            var strContent = JsonConvert.SerializeObject(data);

            var response = await Post(path, strContent, clientId);

            if (!(await ThereAreErrorRequest(response, clientId, secondTrying)))
            {
                return response;
            }
            else
            {
                return await ShipOrderAsync(data, clientId, true);
            }            
        }

        public async Task<HttpResponseMessage> GetShippingDocumentResultAsync(CreateShippingDocumentShopeeRequestDto orderList, int clientId, bool secondTrying = false)
        {
            var path = $"/api/v2/logistics/get_shipping_document_result";

            var strContent = JsonConvert.SerializeObject(orderList);

            var response = await Post(path, strContent, clientId);

            if (!(await ThereAreErrorRequest(response, clientId, secondTrying)))
            {
                return response;
            }
            else
            {
                await GetShippingDocumentResultAsync(orderList, clientId, true);
            }

            return null;
        }

        public async Task<HttpResponseMessage> GetTrackingNumberAsync(string orderSn, int clientId, bool secondTrying = false)
        {
            var path = $"/api/v2/logistics/get_tracking_number";

            string parameters = string.Format("order_sn={0}", orderSn);
            

            var response = await Get(path, parameters, clientId);

            if (!(await ThereAreErrorRequest(response, clientId, secondTrying)))
            {
                return response;
            }
            else
            {
                await GetTrackingNumberAsync(orderSn, clientId, true);
            }

            return null;
        }


        public async Task<HttpResponseMessage> GetShippingParamterAsync(string orderSn, int clientId, bool secondTrying = false)
        {
            var path = $"/api/v2/logistics/get_shipping_parameter";

            string parameters = string.Format("order_sn={0}", orderSn);


            var response = await Get(path, parameters, clientId);

            if (!(await ThereAreErrorRequest(response, clientId, secondTrying)))
            {
                return response;
            }
            else
            {
                await GetShippingParamterAsync(orderSn, clientId, true);
            }

            return null;
        }

        /// <summary>
        /// Get all shipment orders
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<GetShipmentListShiptShopeeResponseDto> GetShipmentList(int clientId, int pageSize = 9999, bool secondTrying = false)
        {
            string path = "/api/v2/order/get_shipment_list";           

            var body = new
            {
                page_size = pageSize
            };

            // Add Params
            string requestBody = JsonConvert.SerializeObject(body);
            
            
            var response = await Post(path, requestBody, clientId);

            if (!(await ThereAreErrorRequest(response, clientId, secondTrying)))
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                var responseReturn = JsonConvert.DeserializeObject<GetShipmentListShiptShopeeResponseDto>(responseBody);

                return responseReturn;
            }
            else
            {
                await GetShipmentList(clientId, pageSize, true);
            }

            return null;            
        }

        protected int GetTimeStamp()
        {
            return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public async Task<bool> ThereAreErrorRequest(HttpResponseMessage response, int clientId, bool secondTrying = false)
        {
            if (response.IsSuccessStatusCode)
            {
                return false;
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (secondTrying)
                    {
                        await NotifyClientToTakeAuthorizationCode(clientId);
                        throw new UnAuthorizedRequestShopeeException("Não foi possivel realizar as requisição, usuário não autorizado. É necessário uma nova autorização.");
                    }
                    else
                    {
                        // try refresh after unauthorized
                        await GetAccessToken(clientId, true);
                    }
                    return true;
                }
                return false;
            }
        }


        public abstract string GetAuthorizationRedirectUrl();
        public abstract string GetHostUrl();
    }
}
