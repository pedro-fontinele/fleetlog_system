using LOGHouseSystem.Adapters.API.MercadoLivre.Response;
using LOGHouseSystem.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Method = RestSharp.Method;

namespace LOGHouseSystem.Adapters.API.MercadoLivre
{
    public class MercadoLivreExtension : IMercadoLivreExtension
    {
        private readonly string ML_URL_BASE = "https://api.mercadolibre.com/";
        private string APP_ID = Environment.MLAppId;
        private string SecretKey = Environment.MLSecretKey;
        private string EndpointUrl = Environment.MLEndpointUrl;
        private string BaseAuthUrl = Environment.MLAuthBaseUrl;
        private string BaseUrl = Environment.MLBaseUrl;
        private string AuthorizationUrlBase;
        private string _accessToken;
        private string _refreshToken;
        private int _mlUserId;


        public MercadoLivreExtension(string accessToken, int mlUserId)
        {
            AuthorizationUrlBase = $"{BaseAuthUrl}?response_type=code&client_id={APP_ID}&redirect_uri={EndpointUrl}&state={"{0}"}";
            _accessToken = accessToken;
            _mlUserId = mlUserId;
        }

        public MercadoLivreExtension()
        {
            AuthorizationUrlBase = $"{BaseAuthUrl}?response_type=code&client_id={APP_ID}&redirect_uri={EndpointUrl}&state={"{0}"}";
        }

        public string GetAuthenticationUrl(string state)
        {
            return string.Format(AuthorizationUrlBase, state);
        }

        public async Task<GetAccessTokenResponse> GetAccessToken(string mlCode)
        {
            var client = new RestClient($"{BaseUrl}");
            var request = new RestRequest("oauth/token", Method.Post);
            request.AddHeader("accept", "application/json");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("client_id", APP_ID);
            request.AddParameter("client_secret", SecretKey);
            request.AddParameter("code", $"{mlCode}");
            request.AddParameter("redirect_uri", $"{EndpointUrl}");

            try
            {
                var res = await client.ExecuteAsync(request);

                if(res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(res.Content);
                }

                GetAccessTokenResponse response = JObject.Parse(res.Content).ToObject<GetAccessTokenResponse>();
                    //client.Post<GetAccessTokenResponse>(request);
                return response;

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public async Task<RestResponse> RefreshAccessToken(string refreshToken)
        {
            var client = new RestClient($"{BaseUrl}");
            var request = new RestRequest("oauth/token", Method.Post);
            request.AddHeader("accept", "application/json");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("client_id", APP_ID);
            request.AddParameter("client_secret", SecretKey);
            request.AddParameter("refresh_token", $"{refreshToken}");
            var response = client.Execute(request);
            return response;
        }

        public async Task<RestResponse> GetLabel(string shippmentId)
        {
            if (string.IsNullOrEmpty(_accessToken)) throw new Exception("Credenciais não configuradas");

            var client = new RestClient($"{BaseUrl}");
            var request = new RestRequest($"shipment_labels?shipment_ids={shippmentId}&response_type=zpl2", Method.Get);

            request.AddHeader("Authorization", "Bearer " + _accessToken);

            RestResponse response = await client.ExecuteAsync(request);

            return response;
        }

        public async Task<RestResponse> GetShippingId(string orderId)
        {
            if (string.IsNullOrEmpty(_accessToken)) throw new Exception("Credenciais não configuradas");

            var client = new RestClient($"{BaseUrl}");
            var request = new RestRequest($"orders/search?seller={_mlUserId}&q={orderId}", Method.Get);
            request.AddHeader("Authorization", "Bearer " + _accessToken);
            RestResponse response = await client.ExecuteAsync(request);            

            return response;
        }

        public async Task<RestResponse> GetShippingIdPack(string orderId)
        {
            if (string.IsNullOrEmpty(_accessToken)) throw new Exception("Credenciais não configuradas");

            var client = new RestClient($"{BaseUrl}");
            var request = new RestRequest($"packs/{orderId}", Method.Get);
            request.AddHeader("Authorization", "Bearer " + _accessToken);
            RestResponse response = await client.ExecuteAsync(request);            

            return response;
        }

        public void SetMercadoLivreConnection(string accessToken, int mlUserId)
        {
            _accessToken = accessToken;            
            _mlUserId = mlUserId;
        }

        public async Task<RestResponse> GetLabelPdf(string shippmentId)
        {
            if (string.IsNullOrEmpty(_accessToken)) throw new Exception("Credenciais não configuradas");

            var client = new RestClient($"{BaseUrl}");
            var request = new RestRequest($"shipment_labels?shipment_ids={shippmentId}&savePdf=Y", Method.Get);

            request.AddHeader("Authorization", "Bearer " + _accessToken);

            RestResponse response = await client.ExecuteAsync(request);

            return response;
        }
    }
}
