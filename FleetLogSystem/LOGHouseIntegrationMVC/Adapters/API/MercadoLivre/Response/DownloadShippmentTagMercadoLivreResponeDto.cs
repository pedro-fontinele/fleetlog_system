using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.API.MercadoLivre.Response
{
    public class DownloadShippmentTagMercadoLivreResponeDto
    {
        [JsonProperty("failed_shipments")]
        public List<FailedShipmentsMercadoLivreResponeDto> FailedShipments { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("error_code")]
        public string ErrorCode { get; set; }
    }

    public class FailedShipmentsMercadoLivreResponeDto
    {
        [JsonProperty("shipment_id")]
        public string ShipmentId { get; set; }

        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("error_code")]
        public string ErrorCode { get; set; }
    }
}
