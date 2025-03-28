using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.API.MercadoLivre.Response
{
    public class MercadoLivreResponse
    {

        [JsonProperty("shipment_id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
