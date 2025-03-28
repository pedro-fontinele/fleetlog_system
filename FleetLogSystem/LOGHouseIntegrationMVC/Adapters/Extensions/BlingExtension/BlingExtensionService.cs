using LOGHouseSystem.Adapters.Extensions.BlingExtension.Dto.Hook.Request;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Newtonsoft.Json;
using RestSharp;

namespace LOGHouseSystem.Adapters.Extensions.BlingExtension
{
    public class BlingExtensionService : IBlingExtensionService
    {
        private string _apikey = null;
        private string _baseUrl = Environment.BlingBaseUrl;
        private IClientsRepository _clientsRepository;
        private IIntegrationRepository _integrationRepository;
        private IIntegrationVariableRepository _integrationVariableRepository;

        public BlingExtensionService(IClientsRepository clientsRepository,
            IIntegrationRepository integrationRepository,
            IIntegrationVariableRepository integrationVariableRepository)
        {
            _clientsRepository = clientsRepository;
            _integrationRepository = integrationRepository;
            _integrationVariableRepository = integrationVariableRepository;
        }

        public async Task SetClientData(int clientId)
        {
            var client = await _clientsRepository.GetByIdAsync(clientId);

            var integrationsData = await _integrationRepository.GetByClientIdAndNameAsync(clientId, BlingIntegrationNames.IntegrationName);

            if (integrationsData == null)
                throw new Exception($"Dados de conexão do Bling do cliente {client.SocialReason} não estão cadastrados.");

            List<IntegrationVariable> integrationsVariables = await _integrationVariableRepository.GetByIntegrationIdAsync(integrationsData.Id);

            if (integrationsVariables.Count < 1)
                throw new Exception($"Dados de conexão do Bling do cliente {client.SocialReason} não estão cadastrados.");

            SetConnection(
                integrationsVariables.Where(e => e.Name == BlingIntegrationNames.ApiKey).FirstOrDefault().Value);
        }

        public async Task<RestResponse> GetXmlByAccessKey(string chaveAcesso)
        {
            if (string.IsNullOrEmpty(_apikey)) throw new Exception("Credenciais não configuradas");

            var client = new RestClient($"{_baseUrl}");
            var request = new RestRequest($"relatorios/nfe.xml.php?chaveAcesso={chaveAcesso}&s", Method.Get);           

            RestResponse response = await client.ExecuteAsync(request);

            return response;
        }

        public async Task<RestResponse> GetInvoiceData(string number, string serial)
        {
            if (string.IsNullOrEmpty(_apikey)) throw new Exception("Credenciais não configuradas");

            var client = new RestClient($"{_baseUrl}");
            var request = new RestRequest($"/Api/v2/notafiscal/{number}/{serial}/json?apikey={_apikey}&number={number}&serie={serial}", Method.Get);

            RestResponse response = await client.ExecuteAsync(request);

            return response;
        }

        public void SetConnection(string accessToken)
        {
            _apikey = accessToken;
        }

        public async Task<RestResponse> GetXmlByXml(string xml)
        {
            if (string.IsNullOrEmpty(_apikey)) throw new Exception("Credenciais não configuradas");

            var client = new RestClient();
            var request = new RestRequest(xml, Method.Get);

            RestResponse response = await client.ExecuteAsync(request);

            return response;
        }

        public async Task<RestResponse> GetOrder(string externalNumber)
        {
            if (string.IsNullOrEmpty(_apikey)) throw new Exception("Credenciais não configuradas");
            var client = new RestClient($"{_baseUrl}");
            var request = new RestRequest($"/Api/v2/pedido/{externalNumber}/json/?apikey={_apikey}", Method.Get);
            RestResponse response = await client.ExecuteAsync(request);

            return response;
        }

        public async Task<RestResponse> GetOrders(DateTime startDate, DateTime endDate, int clientId, int page = 1)
        {
            await SetClientData(clientId);

            var path = $"/Api/v2/pedidos/page={page}/json/?apikey={_apikey}&filters=dataEmissao[{startDate.ToString("dd/MM/yyyy")} TO {endDate.ToString("dd/MM/yyyy")}]";

            if (string.IsNullOrEmpty(_apikey)) throw new Exception("Credenciais não configuradas");
            var client = new RestClient($"{_baseUrl}");
            var request = new RestRequest(path, Method.Get);
            RestResponse response = await client.ExecuteAsync(request);

            return response;
        }

        public async Task<BlingSituacaoPedidoCallbackRequest> GetOrder(string externalNumber, int clientId)
        {
            await SetClientData(clientId);

            var orderResponse = await GetOrder(externalNumber);

            var order = JsonConvert.DeserializeObject<BlingSituacaoPedidoCallbackRequest>(orderResponse.Content);

            return order;
        }
    }
}
