using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Request
{
    public class GetTokenAccountLevelShopeeAccountRequestDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("main_account_id")]
        public int MainAccountId { get; set; }

        [JsonProperty("partner_id")]
        public int PartnerId { get; set; }
    }
}
