using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request
{
    public class ValidatePickingListItemRequest
    {
        [JsonProperty("pickingListId")]
        public int PickingListId { get; set; }

        [JsonProperty("ean")]
        public string Ean { get; set; }
    }
}
