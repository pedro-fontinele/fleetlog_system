using Newtonsoft.Json;

namespace LOGHouseSystem.Models.API.Requests.CaixaMaster
{
    public class CreateCaixaMasterRequest
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("receiptNoteItemId")]
        public int ReceiptNoteItemId { get; set; }
    }
}
