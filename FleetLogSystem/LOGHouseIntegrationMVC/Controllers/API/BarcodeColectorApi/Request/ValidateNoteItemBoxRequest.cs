using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request
{
    public class ValidateNoteItemBoxRequest
    {
        [JsonProperty("boxCode")]
        public string BoxCode { get; set; }
        public int ReceiptNoteId { get; set; }
    }
}
