using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request
{
    public class ValidatePackingItemRequest
    {
        [JsonProperty("pickingListId")]
        public int PackingId { get; set; }

        [JsonProperty("ean")]
        public string Ean { get; set; }
    }
}
