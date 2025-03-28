using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Request
{
    public class GetShipmentListShopeeRequestDto
    {
        [JsonProperty("partner_id")]
        public long PartnerId { get; set; }

        [JsonProperty("shopid")]
        public long ShopId { get; set; }

        [JsonProperty("delivery_ids")]
        public string DeliveryIds { get; set; }
    }
}
