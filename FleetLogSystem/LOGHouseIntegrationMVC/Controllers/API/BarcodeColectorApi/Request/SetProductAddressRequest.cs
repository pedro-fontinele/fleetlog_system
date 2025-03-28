using LOGHouseSystem.Models;
using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request
{
    public class SetProductAddressRequest
    {
        [JsonProperty("productId")]
        public int ProductId { get; set; }

        [JsonProperty("addressingPositionName")]
        public string? AddressingPositionName { get; set; }

        [JsonProperty("ReceiptNoteItem")]
        public int ReceiptNoteItemID { get; set; }
    }
}
