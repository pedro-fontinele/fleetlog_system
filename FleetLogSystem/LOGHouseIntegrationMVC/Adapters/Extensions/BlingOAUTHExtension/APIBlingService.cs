using LOGHouseSystem.Adapters.BaseOAUTH2Extension.Dto;
using LOGHouseSystem.Adapters.Extensions.BaseOAUTH2Extension;
using LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto;
using LOGHouseSystem.Infra.Enum;
using LOGHouseSystem.Models;
using LOGHouseSystem.Models.SendEmails;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services;
using LOGHouseSystem.Services.Interfaces;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension
{
    public class APIBlingService : BaseOAUTH2BaseService, IAPIBlingService
    {
        private string HOST;
        private IClientsRepository _clientsRepository;
        private IEmailService _emailService;
        private IDataBlingService _dataBlingService;
        private IIntegrationVariableRepository _integrationVariableRepository;

        public APIBlingService(IDataBlingService dataBlingService,
            IClientsRepository clientsRepository,
            IEmailService emailService,
            IIntegrationVariableRepository integrationVariableRepository) : base(dataBlingService)
        {
            PartnerName = "Bling V3";
            HOST = Environment.BlingV3BaseUrl;
            _clientsRepository = clientsRepository;
            _emailService = emailService;
            _dataBlingService = dataBlingService;
            _integrationVariableRepository = integrationVariableRepository;
        }

        private async Task<DataBlingV3Dto> GetDataByClinetId(int clientId)
        {
            var data = await _dataBlingService.GetData(clientId);
            
            if (data == null)
            {
                throw new AuthenticationNotFoundException(string.Format("Dados do parceiro da {0} não estão configurados.", PartnerName));
            }

            return data;
        }

        public override async Task<string> GenerateAuthorizationCodeUrl(int clientId)
        {
            return Environment.BlingV3BaseUrl + "Api/v3/oauth/authorize?response_type=code&client_id=[BLING_CLIENT_ID]&state=" + clientId.ToString();
        }

        public override async Task<AuthenticationAPIResponseDto> GetAccessTokenByCode(string code, int clientId)
        {
            var data = await GetDataByClinetId(clientId);

            string path = "/Api/v3/oauth/token";

            RestClient client = new RestClient(HOST);
            RestRequest request = new RestRequest(path, Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            //request.AddHeader("Accept", "1.0");

            var textBytes = System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", data.BlingClientId, data.BlingClientSecret));
            var base64 =  System.Convert.ToBase64String(textBytes);

            request.AddHeader("Authorization", string.Format("Basic {0}", base64));

            //request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("code", code);

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

        protected override async Task RequestUserAuthorizationCode(string url, int clientId)
        {
            var client = await _clientsRepository.GetByIdAsync(clientId);

            _emailService.SendEmail(new EmailData
            {
                EmailBody = $@"Integração com o Bling requisitando autenticação para o cliente {client.SocialReason} - {client.Cnpj}, pare realizar, <a href='{url}'>Clique Aqui</a>.",
                EmailSubject = $"Bling - Autenticação necessária {client.SocialReason} - {client.Cnpj}",
                EmailToId = Environment.MelhorEnvioEnvironment.NotificationEmail,
                EmailToName = Environment.MelhorEnvioEnvironment.NotificationEmail
            }, null, client.Id);
        }

        protected override async Task<AuthenticationDto> TakeAccessTokenByRefreshToken(string refreshToken, int clientId)
        {

            var requestDto = await GetAccesTokenByRefreshToken(refreshToken, clientId);
            return ConvertResponseToDto(requestDto, clientId);
        }

        public async Task<AuthenticationAPIResponseDto> GetAccesTokenByRefreshToken(string code, int clientId)
        {
            var data = await GetDataByClinetId(clientId);

            string path = "/Api/v3/oauth/token";

            RestClient client = new RestClient(HOST);
            RestRequest request = new RestRequest(path, Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            var textBytes = System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", data.BlingClientId, data.BlingClientSecret));
            var base64 = System.Convert.ToBase64String(textBytes);

            request.AddHeader("Authorization", string.Format("Basic {0}", base64));

            /*var body = new AuthenticationBlingRequestDto()
            {
                GrantType = "refresh_token",                
                RefreshToken = code
            };*/

            //request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("refresh_token", code);

            var response = await client.ExecuteAsync(request);
            AuthenticationAPIResponseDto responseContent = JsonConvert.DeserializeObject<AuthenticationAPIResponseDto>(response.Content);

            return responseContent;
        }

        public async Task CreateV3IntegrationVariables(int integrationId, int clientId)
        {

            List<IntegrationVariable> integrationVariables = new List<IntegrationVariable>();

            string[] integrationVariableNames = new string[] {
                BlingV3IntegrationNames.BlingClientId,
                BlingV3IntegrationNames.BlingSecret,
                BlingV3IntegrationNames.AccessToken,
                BlingV3IntegrationNames.AccessCreatedAt,
                BlingV3IntegrationNames.RefreshToken,
                BlingV3IntegrationNames.ExpiresIn,
                BlingV3IntegrationNames.Url
            };

            foreach (string variable in integrationVariableNames)
            {

                var value = BlingV3IntegrationNames.Url == variable ? await GenerateAuthorizationCodeUrl(clientId) : "";

                integrationVariables.Add(new IntegrationVariable
                {
                    IntegrationId = integrationId,
                    Name = variable,
                    Value = value
                });
            }

            await _integrationVariableRepository.AddRangeAsync(integrationVariables);
        }

        public async Task<RestResponse?> SendNfe(BlingNfeRequestDto nfe, int clientId, bool secondTrying = false)
        {
            var data = await GetAccessToken(clientId);

            string path = "/Api/v3/nfe";

            RestClient client = new RestClient(HOST);
            RestRequest request = new RestRequest(path, Method.Post);
            request.AddHeader("Content-Type", "application/json");

            request.AddHeader("Authorization", string.Format("Bearer {0}", data.AccessToken));

            request.AddParameter("application/json", JsonConvert.SerializeObject(nfe), ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);

            if (!await ThereAreErrorRequest(response, clientId, secondTrying))
            {
                return response;
            }
            else
            {
                return await SendNfe(nfe, clientId, true);
            }
        }

        public async Task<RestResponse?> ConfirmNfe(long idNfe, int clientId, bool secondTrying = false)
        {
            var data = await GetAccessToken(clientId);

            string path = string.Format("/Api/v3/nfe/{0}/enviar", idNfe.ToString());

            RestClient client = new RestClient(HOST);
            RestRequest request = new RestRequest(path, Method.Post);
            request.AddHeader("Content-Type", "application/json");

            request.AddHeader("Authorization", string.Format("Bearer {0}", data.AccessToken));

            var response = await client.ExecuteAsync(request);

            if (!await ThereAreErrorRequest(response, clientId, secondTrying))
            {
                return response;
            }
            else
            {
                return await ConfirmNfe(idNfe, clientId, true);
            }
        }

        public async Task<RestResponse?> GetEnvironmentOperation(string description, int clientId, bool secondTrying = false)
        {
            var data = await GetAccessToken(clientId);

            string path = string.Format("/Api/v3/naturezas-operacoes?descricao={0}", description);

            RestClient client = new RestClient(HOST);
            RestRequest request = new RestRequest(path, Method.Get);
            //request.AddHeader("Content-Type", "application/json");

            request.AddHeader("Authorization", string.Format("Bearer {0}", data.AccessToken));

            var response = await client.ExecuteAsync(request);

            if (!await ThereAreErrorRequest(response, clientId, secondTrying))
            {
                return response;
            }
            else
            {
                return await GetEnvironmentOperation(description, clientId, true);
            }
            
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
                        throw new Exception("Não foi possivel realizar as requisição, usuário não autorizado. É necessário uma nova autorização.");
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

        public async Task<RestResponse?> UpdateNfe(BlingNfeRequestDto nfe, long idNfe, int clientId, bool secondTrying = false)
        {
            var data = await GetAccessToken(clientId);

            string path = string.Format("/Api/v3/nfe/{0}", idNfe.ToString());

            RestClient client = new RestClient(HOST);
            RestRequest request = new RestRequest(path, Method.Put);
            request.AddHeader("Content-Type", "application/json");

            request.AddHeader("Authorization", string.Format("Bearer {0}", data.AccessToken));

            request.AddParameter("application/json", JsonConvert.SerializeObject(nfe), ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);

            if (!await ThereAreErrorRequest(response, clientId, secondTrying))
            {
                return response;
            }
            else
            {
                return await UpdateNfe(nfe, idNfe, clientId, true);
            }
        }

        public async Task<RestResponse> GetOrders(DateTime startDate, DateTime endDate, int clientId, bool secondTrying = false)
        {
            var data = await GetAccessToken(clientId);

            string path = $"/pedidos/vendas?dataInicial={startDate.ToString("yyyy-mm-dd")}&dataFinal={endDate.ToString("yyyy-mm-dd")}";

            RestClient client = new RestClient(HOST);
            RestRequest request = new RestRequest(path, Method.Get);           

            request.AddHeader("Authorization", string.Format("Bearer {0}", data.AccessToken));            

            var response = await client.ExecuteAsync(request);

            if (!await ThereAreErrorRequest(response, clientId, secondTrying))
            {
                return response;
            }
            else
            {
                return await GetOrders(startDate, endDate, clientId, true);
            }
        }

        public async Task<RestResponse> GetOrder(string orderNumber, int clientId, bool secondTrying = false)
        {
            var data = await GetAccessToken(clientId);

            string path = $"/pedidos/vendas/{orderNumber}";

            RestClient client = new RestClient(HOST);
            RestRequest request = new RestRequest(path, Method.Get);            
            request.AddHeader("Authorization", string.Format("Bearer {0}", data.AccessToken));

            var response = await client.ExecuteAsync(request);

            if (!await ThereAreErrorRequest(response, clientId, secondTrying))
            {
                return response;                
            }
            else
            {
                return await GetOrder(orderNumber, clientId, true);
            }
        }

        public async Task<RestResponse> GetNoteByInvoiceId(string invoiceId, int clientId, bool secondTrying = false)
        {
            var data = await GetAccessToken(clientId);

            var path = $"/Api/v3/nfe/{invoiceId}";
            RestClient client = new RestClient(HOST);
            RestRequest request = new RestRequest(path, Method.Post);
            request.AddHeader("Content-Type", "application/json");

            request.AddHeader("Authorization", string.Format("Bearer {0}", data.AccessToken));

            var response = client.Execute(request);

            if (!await ThereAreErrorRequest(response, clientId, secondTrying))
            {
                return response;                                
            }
            else
            {
                return await GetNoteByInvoiceId(invoiceId, clientId, true);
            }
        }
        public async Task<RestResponse> GetNotesBySearch(string blingId, int clientId, bool secondTrying = false)
        {
            var data = await GetAccessToken(clientId);

            string path = $"/Api/v3/nfe?numeroLoja={blingId}";

            RestClient client = new RestClient(HOST);
            RestRequest request = new RestRequest(path, Method.Post);
            request.AddHeader("Content-Type", "application/json");

            request.AddHeader("Authorization", string.Format("Bearer {0}", data.AccessToken));

            var response = client.Execute(request);

            if (!await ThereAreErrorRequest(response, clientId, secondTrying))
            {
                return response;                
            }
            else
            {
                return await GetNotesBySearch(blingId, clientId, true);
            }
        }

        public async Task<RestResponse> GetNFe(string id, int clientId, bool secondTrying = false)
        {
            var data = await GetAccessToken(clientId);

            string path = $"/Api/v3/nfe/{id}";

            RestClient client = new RestClient(HOST);
            RestRequest request = new RestRequest(path, Method.Get);
            request.AddHeader("Content-Type", "application/json");

            request.AddHeader("Authorization", string.Format("Bearer {0}", data.AccessToken));

            var response = client.Execute(request);

            if (!await ThereAreErrorRequest(response, clientId, secondTrying))
            {
                return response;
            }
            else
            {
                return await GetNFe(id, clientId, true);
            }
        }
    }
}
