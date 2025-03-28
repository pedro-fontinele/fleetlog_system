using LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Responses;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace LOGHouseSystem.Adapters.Extensions.SmartgoExtension
{
    public class SmartgoExtension
    {
        /*private readonly string API_KEY = "jBY6jSbpQnS3vlmI6MPEUEGCJAKRwC+H4I83fVfNG6L3pKU3mtN2DM4DIdYRAV3FjfVjN7RS2CVloKSrXTVkx60VpqDoNEaCrJZN+6cPAG0=";
        private readonly string URL_BASE = "https://apigateway.smartgo.com.br";*/

        private readonly string API_KEY = "";
        private readonly string URL_BASE = "";

        public SmartgoExtension()
        {
                API_KEY = Environment.SmartGoConfiguration.ApiKey;
                URL_BASE = Environment.SmartGoConfiguration.Host;
        }
        public async Task<SaldoSimplificadoResponse> GetSaldoSimlpificadoPorDepositante(int idDepositante, int page = 1)
        {
            string url = $"{URL_BASE}";
            var options = new RestClientOptions(url)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest($"/estoque/saldo?IdDepositante={idDepositante}&page={page}", Method.Get);
            request.AddHeader("api_key", API_KEY);

            RestResponse response = await client.ExecuteAsync(request);
            if (!response.IsSuccessStatusCode) throw new Exception("Falha ao buscar saldo do depositante");

            SaldoSimplificadoResponse responseModel = JObject.Parse(response.Content).ToObject<SaldoSimplificadoResponse>();

            return responseModel;
        }

        public async Task<SaldoDetalhadoResponse> GetSaldoDetalhadoPorDepositante(int idDepositante)
        {
            string url = $"{URL_BASE}";
            var options = new RestClientOptions(url)
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest($"/estoque/saldo/detalhado?IdDepositante={idDepositante}", Method.Get);
            request.AddHeader("api_key", API_KEY);

            RestResponse response = await client.ExecuteAsync(request);
            if (!response.IsSuccessStatusCode) throw new Exception("Falha ao buscar saldo detalhado do depositante");

            SaldoDetalhadoResponse responseModel = JObject.Parse(response.Content).ToObject<SaldoDetalhadoResponse>();

            return responseModel;
        }
    }
}
