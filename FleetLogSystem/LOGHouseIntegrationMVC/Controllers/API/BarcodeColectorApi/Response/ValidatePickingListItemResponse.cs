using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.API.BarcodeColectorApi.Response
{
    public class ValidatePickingListItemResponse
    {
        [JsonProperty("pickingListItem")]
        public PickingListItem PickingListItem { get; set; }

        [JsonProperty("pickingListStatus")]
        public PickingListStatus PickingListStatus { get; set; }
    }
}
