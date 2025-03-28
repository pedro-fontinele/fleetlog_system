using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.API.BarcodeColectorApi.Response
{
    public class ValidatePackingItemResponse
    {
        [JsonProperty("packingItem")]
        public PackingItem PackingItem { get; set; }

        [JsonProperty("packingStatus")]
        public PackingStatus PackingStatus { get; set; }
    }
}
