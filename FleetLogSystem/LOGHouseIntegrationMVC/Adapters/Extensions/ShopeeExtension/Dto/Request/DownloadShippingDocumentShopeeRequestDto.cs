using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Request
{
    public class DownloadShippingDocumentShopeeRequestDto
    {
        [JsonProperty("shipping_document_type")]
        public string ShippingDocumentType { get; set; }

        [JsonProperty("order_list")]
        public List<ShipmentListShiptShopeeDto> OrderList { get; set; } = new List<ShipmentListShiptShopeeDto>();
    }
}
