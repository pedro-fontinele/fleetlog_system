using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Request
{
    public class CreateShippingDocumentShopeeRequestDto
    {
        [JsonProperty("order_list")]
        public List<CreateShipmentListShiptShopeeDto> OrderList { get; set; } = new List<CreateShipmentListShiptShopeeDto>();        

    }

    public class CreateShipmentListShiptShopeeDto
    {
        [JsonProperty("order_sn")]
        public string OrderNumber { get; set; }

        [JsonProperty("package_number")]
        public string PackageNumber { get; set; }

        [JsonProperty("shipping_document_type")]
        public string ShippingDocumentType { get; set; }

        [JsonProperty("tracking_number")]
        public string TrackingNumber { get; set; }
    }
}
