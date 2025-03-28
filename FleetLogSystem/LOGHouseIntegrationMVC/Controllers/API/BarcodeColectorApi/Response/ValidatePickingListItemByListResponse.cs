using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request;
using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.API.BarcodeColectorApi.Response
{
    public class ValidatePickingListItemByListResponse
    {
        [JsonProperty("sent")]
        public int Sent { get; set; }

        [JsonProperty("success")]
        public int Success { get; set; }

        [JsonProperty("error")]
        public List<ValidatePickingListItemRequest> Error { get; set; }
    }
}
