using LOGHouseSystem.Adapters.Extensions.Kangu.Dto;
using LOGHouseSystem.Adapters.Extensions.NFeExtension;
using Newtonsoft.Json;
using RestSharp;

namespace LOGHouseSystem.Adapters.Extensions.Kangu
{
    public class KanguExtensionService : IKanguExtensionService
    {
        private string _apikey = null;
        private string _baseUrl = Environment.KanguBaseUrl;
        public KanguExtensionService()
        {

        }

        public async Task<RestResponse> GetTag(string order)
        {
            if (string.IsNullOrEmpty(_apikey)) throw new Exception("Credenciais não configuradas");

            var client = new RestClient($"{_baseUrl}");
            client.AddDefaultHeader("token", _apikey);
            var request = new RestRequest($"/imprimir-etiqueta/{order}", Method.Get);

            RestResponse response = await client.ExecuteAsync(request);

            return response;
        }

        public async Task<RestResponse> PostTag(KanguPostTagRequestDto tag)
        {
            if (string.IsNullOrEmpty(_apikey)) throw new Exception("Credenciais não configuradas");

            var client = new RestClient($"{_baseUrl}");
            client.AddDefaultHeader("token", _apikey);
            var request = new RestRequest($"/imprimir-etiqueta-lote", Method.Post);
            request.AddParameter("application/json", JsonConvert.SerializeObject(tag), ParameterType.RequestBody);

            RestResponse response = await client.ExecuteAsync(request);

            return response;
        }

        public void SetConnection(string accessToken)
        {
            _apikey = accessToken;
        }
    }
}
