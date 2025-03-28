using LOGHouseSystem.Models;
using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request
{
    public class PackingListTransportationRequest
    {
        [JsonProperty("transportationPersonId")]
        public int? TransportationPersonId { get; set; }

        [JsonProperty("shippingCompanyId")]
        public int? ShippingCompanyId { get; set; }

        [JsonProperty("vehiclePlate")]
        public string? VehiclePlate { get; set; }

        [JsonProperty("observation")]
        public string? Observation { get; set; }
    }
}
