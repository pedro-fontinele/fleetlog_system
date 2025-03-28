using LOGHouseSystem.Adapters.Extensions.DeOlhoNoImposto;
using LOGHouseSystem.Services.Integrations;
using LOGHouseSystem.Services.Interfaces;
using Newtonsoft.Json;

namespace LOGHouseSystem.Services
{
    public class DeOlhoNoImpostoIntegration : IDeOlhoNoImpostoIntegration
    {
        public DeOlhoNoImpostoIntegration()
        {

        }
        public async Task<DeOlhoNoImpostoResponse> GetTax(string codigo, string uf, string cnpj, string descricao, string unidadeMedida, string valor, string gtin)
        {
            try
            {
                string baseUrl = "https://deolhonoimposto.ibpt.org.br/";
                string endpoint = string.Format("api/v1/produtos?codigo{0}&uf={1}&cnpj{2}&descricao={3}&unidadeMedida={4}&valor={5}&gtin={6}", codigo, uf, codigo, descricao, unidadeMedida, valor, gtin);

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(baseUrl);
                    
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Environment.DeOlhoNoImpostoToken}");

                    HttpResponseMessage response = await httpClient.GetAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<DeOlhoNoImpostoResponse>(responseBody);
                        
                    }
                    else
                    {
                        Log.Error("Não foi possível buscar os dados do De Olho no Imposto. " + await response.Content.ReadAsStringAsync());
                        throw new Exception("Não foi possível buscar os dados do De Olho no Imposto. " + await response.Content.ReadAsStringAsync());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
