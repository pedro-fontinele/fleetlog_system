using LOGHouseSystem.Adapters.Extensions.NFeExtension;
using LOGHouseSystem.Adapters.Extensions.TinyExtension.Dto;
using LOGHouseSystem.Adapters.Extensions.TinyExtension.Exceptions;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Tiny.Dto;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace LOGHouseSystem.Adapters.Extensions.TinyExtension.Service
{
    public class TinyAPIService : ITinyAPIService
    {
        private IIntegrationRepository _integrationRepository;
        private IIntegrationVariableRepository _integrationVariableRepository;
        private INFeExtension _nfeExtension;

        public TinyAPIService(IIntegrationRepository integrationRepository, IIntegrationVariableRepository integrationVariableRepository, INFeExtension nfeExtension)
        {
            _integrationRepository = integrationRepository;
            _integrationVariableRepository = integrationVariableRepository;
            _nfeExtension = nfeExtension;
        }

        private async Task<IntegrationVariable> GetApiKey(Client client)
        {
            var integration = await _integrationRepository.GetByClientIdAndNameAsync(client.Id, TinyIntegationNames.IntegrationName);

            if (integration == null)
                throw new NotFoundDataConnectionTinyException($"Dados de conexão do cliente {client.Id} não estão cadastrados");

            var itens = await _integrationVariableRepository.GetByIntegrationIdAndNameAsync(integration.Id, TinyIntegationNames.ApiKey);

            if (itens != null)
            {
                return itens;
            }
            else
            {
                var message = string.Format("API Key do cliente {0} não encontrado no cadastro de variaveis.", client.Id);
                Log.Error(message);
                throw new Exception(message);
            }
        }

        public async Task<TinyCompleteOrderRequestDto> GetOrder(string id, Client client)
        {
            var apiKey = await GetApiKey(client);

            string url = "https://api.tiny.com.br/api2/pedido.obter.php";
            string token = apiKey.Value;
            string formato = "JSON";
            string data = "token=" + token + "&id=" + id.ToString() + "&formato=" + formato;

            string result = await enviarRESTPOSTAsync(url, data);
            return JsonConvert.DeserializeObject<TinyCompleteOrderRequestDto>(result);
        }

        public async Task<TinyInvoiceResponse> GetInvoice(string id, Client client)
        {
            var apiKey = await GetApiKey(client);

            string url = "https://api.tiny.com.br/api2/nota.fiscal.obter.php";
            string token = apiKey.Value;
            string formato = "JSON";
            string data = "token=" + token + "&id=" + id.ToString() + "&formato=" + formato;

            string result = await enviarRESTPOSTAsync(url, data);
            return JsonConvert.DeserializeObject<TinyInvoiceResponse>(result);
        }

        public async Task<TinyCompleteProductDto> GetProduct(long id, Client client)
        {
            var apiKey = await GetApiKey(client);

            string url = "https://api.tiny.com.br/api2/produto.obter.php";
            string token = apiKey.Value;
            string formato = "JSON";
            string data = "token=" + token + "&id=" + id.ToString() + "&formato=" + formato;

            string result = await enviarRESTPOSTAsync(url, data);
            return JsonConvert.DeserializeObject<TinyCompleteProductDto>(result);
        }

        public async Task<string?> GetXmlInvoice(int idTinyInvoice, Client client)
        {

            const string beforeXmlString = "<retorno><status_processamento>3</status_processamento><status>OK</status><xml_nfe>";
            const string afterXmlString = "</nfeProc>";
            var apiKey = await GetApiKey(client);

            string url = "https://api.tiny.com.br/api2/nota.fiscal.obter.xml.php";
            string token = apiKey.Value;
            string formato = "JSON";
            string data = "token=" + token + "&id=" + idTinyInvoice.ToString() + "&formato=" + formato;

            string result = await enviarRESTPOSTAsync(url, data);
            
            if (!result.Contains(beforeXmlString))
            {
                return null;
            }

            string returned = result.Replace(beforeXmlString, "");

            int index = returned.IndexOf(afterXmlString);
            if (index >= 0) {
                returned = returned.Substring(0, index + afterXmlString.Length);
            }
            else
            {
                return null;
            }

            return returned;
        }

        public async Task<string> enviarRESTPOSTAsync(string url, string data, string optional_headers = null)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            if (optional_headers != null)
            {
                request.Headers[HttpRequestHeader.Authorization] = optional_headers;
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            request.ContentLength = byteArray.Length;

            using (var stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(byteArray, 0, byteArray.Length);
            }

            string responseContent = null;
            try
            {
                using (var response = await request.GetResponseAsync() as HttpWebResponse)
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = await streamReader.ReadToEndAsync();
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)ex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            responseContent = await reader.ReadToEndAsync();
                        }
                    }
                }
            }

            return responseContent;
        }        
    }
}
