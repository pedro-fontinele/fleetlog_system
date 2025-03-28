using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Request
{
    public class GetTokenAccountLevelShopeeRequestDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("shop_id")]
        public int ShopId { get; set; }

        [JsonProperty("partner_id")]
        public int PartnerId { get; set; }
    }
}
