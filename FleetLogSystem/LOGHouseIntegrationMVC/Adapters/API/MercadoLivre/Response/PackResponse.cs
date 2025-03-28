using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.API.MercadoLivre.Response
{
    public class PackResponse
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("shipment")]
        public Shipment Shipment { get; set; }

    }

    public class Shipment
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }
}
