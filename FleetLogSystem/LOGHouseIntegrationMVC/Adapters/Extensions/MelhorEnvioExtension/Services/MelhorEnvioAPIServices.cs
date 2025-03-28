using LOGHouseSystem.Adapters.BaseOAUTH2Extension.Dto;
using LOGHouseSystem.Adapters.Extensions.BaseOAUTH2Extension;
using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension.Dto;
using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension.Dto.Request;
using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension.Dto.Response;
using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension.Exceptions;
using LOGHouseSystem.Infra.Enum;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension
{
    public abstract class MelhorEnvioAPIServices : BaseOAUTH2BaseService, IMelhorEnvioAPIServices
    {        
        private List<DataMelhorEnvioDto> _dataMelhorEnvioDto = new List<DataMelhorEnvioDto>();
        public string HOST = "";
        private string MEClientId = "";

        private IDataMelhorEnvioService _dataMelhorEnvioService;

        public MelhorEnvioAPIServices(IDataMelhorEnvioService dataMelhorEnvioService) : base(dataMelhorEnvioService)
        {
            HOST = Environment.MelhorEnvioEnvironment.BaseUrl;
            MEClientId = Environment.MelhorEnvioEnvironment.ClientId;
            _dataMelhorEnvioService = dataMelhorEnvioService;
            PartnerName = "Melhor Envio";
            REFRESH_TOKEN_TIME = 45 * 24 * 60 * 60; // days * hours * minutes * seconds
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        /// <exception cref="ShopDataNotFoundException"></exception>
        private async Task<DataMelhorEnvioDto> GetDataByClinetId(int clientId)
        {
            var data = _dataMelhorEnvioDto.Where(e => e.ClientId == clientId).FirstOrDefault();

            // if null, try to get from database
            if (data == null)
            {
                data = await _dataMelhorEnvioService.GetData(clientId);
            }

            // if isn't null, add to local variable
            if (data != null)
            {
                _dataMelhorEnvioDto.Add(data);
            }

            // if isn't founded, trigger an exception
            if (data == null)
            {
                throw new AuthenticationNotFoundException(string.Format("Dados do parceiro da {0} não estão configurados.", PartnerName));
            }

            return data;
        }

        public async override Task<string> GenerateAuthorizationCodeUrl(int clientId)
        {

            // Permission
            var scope = "cart-read cart-write companies-read companies-write coupons-read coupons-write notifications-read orders-read products-read products-write purchases-read shipping-calculate shipping-cancel shipping-checkout shipping-companies shipping-generate shipping-preview shipping-print shipping-share shipping-tracking ecommerce-shipping transactions-read users-read users-write";       
            var data = await GetDataByClinetId(clientId);
            var host = HOST;
            var path = "/oauth/authorize";
            var redirectUrl = GetAuthorizationRedirectUrl();
            var url = $"{host}{path}?client_id={data.MelhorEnvioClientId}&redirect_uri={redirectUrl}&response_type=code&scope={scope}&state={clientId}";

            return url;
        }

        public string GetBaseUrlAuthorizationCode(int clientId)
        {
            var scope = "cart-read cart-write companies-read companies-write coupons-read coupons-write notifications-read orders-read products-read products-write purchases-read shipping-calculate shipping-cancel shipping-checkout shipping-companies shipping-generate shipping-preview shipping-print shipping-share shipping-tracking ecommerce-shipping transactions-read users-read users-write";            
            var host = HOST;
            var path = "/oauth/authorize";
            var redirectUrl = GetAuthorizationRedirectUrl();
            var url = $"{host}{path}?client_id={MEClientId}&redirect_uri={redirectUrl}&response_type=code&scope={scope}&state={clientId}";

            return url;
        }

        /// <summary>
        /// Take the access token by refresh token
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override async Task<AuthenticationDto> TakeAccessTokenByRefreshToken(string refreshToken, int clientId)
        {            

            var requestDto = await GetAccesTokenByRefreshToken(refreshToken, clientId);
            return ConvertResponseToDto(requestDto, clientId);
        }

        public async Task<AuthenticationAPIResponseDto> GetAccesTokenByRefreshToken(string code, int clientId)
        {
            var data = await GetDataByClinetId(clientId);
            string path = "/oauth/token";

            RestClient client = new RestClient(HOST);
            RestRequest request = new RestRequest(path, Method.Post);
            request.AddHeader("Content-Type", "application/json");

            // required information on the API
            request.AddHeader("User-Agent", string.Format("Aplicacao {0}", GetContactEmail()));


            var body = new AuthenticationMelhorEnvioRequestDto()
            {
                ClientId = data.MelhorEnvioClientId,
                ClientSecret = data.MelhorEnvioSecret,
                GrantType = "refresh_token",
                RedirectUri = GetAuthorizationRedirectUrl(),
                RefreshToken = code
            };

            request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);
            AuthenticationAPIResponseDto responseContent = JsonConvert.DeserializeObject<AuthenticationAPIResponseDto>(response.Content);

            return responseContent;
        }

        public async override Task<AuthenticationAPIResponseDto> GetAccessTokenByCode(string code, int clientId)
        {
            var data = await GetDataByClinetId(clientId);

            string path = "/oauth/token";            

            RestClient client = new RestClient(HOST);
            RestRequest request = new RestRequest(path, Method.Post);
            request.AddHeader("Content-Type", "application/json");

            var body = new AuthenticationMelhorEnvioRequestDto()
            {
                ClientId = data.MelhorEnvioClientId,
                ClientSecret = data.MelhorEnvioSecret,
                GrantType = "authorization_code",
                RedirectUri = GetAuthorizationRedirectUrl(),
                Code = code
            };

            request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessStatusCode)
            {
                AuthenticationAPIResponseDto responseContent = JsonConvert.DeserializeObject<AuthenticationAPIResponseDto>(response.Content);

                return responseContent;
            }
            else
            {
                throw new Exception($"Houve um erro ao tentar gerar um novo Token: {response.Content}");
            }
            
        }


        /// <summary>
        /// The API is able to send a order list but the response is just one URL
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<GetShippingDataMelhorEnvioDtoResponse> GetShippingData(List<string> order, int clientId, bool secondTrying = false)
        {
            var data = await GetAccessToken(clientId);

            string path = "/api/v2/me/shipment/print";

            RestClient client = new RestClient(HOST);
            RestRequest request = new RestRequest(path, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            // required information on the API
            request.AddHeader("User-Agent", string.Format("Aplicacao {0}", GetContactEmail()));
            request.AddHeader("Authorization", $"Bearer {data.AccessToken}");

            var body = new GetShippingDataMelhorEnvioDtoRequest()
            {
                Mode = "public",
                Orders = order
            };

            request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);

            if (!(await ThereAreErrorRequest(response, clientId, secondTrying)))
            {                
                GetShippingDataMelhorEnvioDtoResponse responseContent = JsonConvert.DeserializeObject<GetShippingDataMelhorEnvioDtoResponse>(response.Content);

                return responseContent;
            }
            else
            {
                await GetTagInformation(order[0], clientId, true);
            }

            return null;
        }

        /// <summary>
        ///  Return order by state
        /// </summary>
        /// <param name="state"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<GetAllShippingMelhorEnvioDtoResponse> GetAllShippingByState(string state, int clientId, bool secondTrying = false)
        {

            var data = await GetAccessToken(clientId);                        

            string path = string.Format("/api/v2/me/orders?status={0}&per_page=100000", state);

            RestClient client = new RestClient(HOST);
            RestRequest request = new RestRequest(path, Method.Get);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {data.AccessToken}");
            // required information on the API
            request.AddHeader("User-Agent", string.Format("Aplicacao {0}", GetContactEmail()));
            var response = await client.ExecuteAsync(request);

            if (!(await ThereAreErrorRequest(response, clientId, secondTrying)))
            {
                GetAllShippingMelhorEnvioDtoResponse responseContent = JsonConvert.DeserializeObject<GetAllShippingMelhorEnvioDtoResponse>(response.Content);

                return responseContent;
            }
            else
            {
                await GetAllShippingByState(state, clientId, true);
            }

            return null;
        }

        public async Task<GetTagMelhorEnvioResponse> GetTagInformation(string tag, int clientId, bool secondTrying = false)
        {
            var data = await GetAccessToken(clientId);

            string path = string.Format("/api/v2/me/orders/{0}", tag);

            RestClient client = new RestClient(HOST);
            RestRequest request = new RestRequest(path, Method.Get);
            request.AddHeader("Content-Type", "application/json");
            // required information on the API
            request.AddHeader("User-Agent", string.Format("Aplicacao {0}", GetContactEmail()));
            request.AddHeader("Authorization", $"Bearer {data.AccessToken}");
            var response = await client.ExecuteAsync(request);

            if (!(await ThereAreErrorRequest(response, clientId, secondTrying)))
            {
                GetTagMelhorEnvioResponse responseContent = JsonConvert.DeserializeObject<GetTagMelhorEnvioResponse>(response.Content);

                return responseContent;
            }
            else
            {
                await GetTagInformation(tag, clientId, true);
            }

            return null;
        }

        public async Task<bool> ThereAreErrorRequest(RestResponse response, int clientId, bool sencondTrying)
        {
            if (response.IsSuccessStatusCode)
            {
                return false;
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (sencondTrying)
                    {
                        await NotifyClientToTakeAuthorizationCode(clientId);
                        throw new UnAuthorizedRequestMelhorEnvioException("Não foi possivel realizar as requisição, usuário não autorizado. É necessário uma nova autorização.");
                    }
                    else
                    {
                        // try refresh after unauthorized
                        GetAccessToken(clientId, true);
                    }

                    return true;
                    
                }
                return false;
            }
        }

        /// <summary>
        /// Get all shipping by a name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public async Task<GetAllShippingMelhorEnvioDtoResponse> GetAllShippingByName(string? name, int clientId, bool secondTrying = false)
        {

            var data = await GetAccessToken(clientId);

            string path = string.Format("/api/v2/me/orders/search?q={0}", name);

            RestClient client = new RestClient(HOST);
            RestRequest request = new RestRequest(path, Method.Get);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {data.AccessToken}");
            // required information on the API
            request.AddHeader("User-Agent", string.Format("Aplicacao {0}", GetContactEmail()));
            var response = await client.ExecuteAsync(request);

            if (!(await ThereAreErrorRequest(response, clientId, secondTrying)))
            {
                GetAllShippingMelhorEnvioDtoResponse responseContent = JsonConvert.DeserializeObject<GetAllShippingMelhorEnvioDtoResponse>(response.Content);

                return responseContent;
            }
            else
            {
                await GetAllShippingByName(name, clientId, true);
            }

            return null;
        }

        protected abstract string GetContactEmail();

        public abstract string GetAuthorizationRedirectUrl();
    }
}
