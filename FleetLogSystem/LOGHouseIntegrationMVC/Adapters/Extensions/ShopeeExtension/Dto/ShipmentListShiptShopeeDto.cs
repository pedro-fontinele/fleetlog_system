using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto
{
    public class ShipmentListShiptShopeeDto
    {
        [JsonProperty("order_sn")]
        public string OrderNumber { get; set; }

        [JsonProperty("package_number")]
        public string PackageNumber { get; set; }
    }
}
