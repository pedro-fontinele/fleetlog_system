using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request
{
    public class ValidateNoteItemRequest
    {
        [JsonProperty("receiptNoteId")]
        public int ReceiptNoteId { get; set; }

        [JsonProperty("ean")]
        public string Ean { get; set; }
    }
}
