using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Request
{
    public class AuthorizationCodeShopeeRequestDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("main_account_id")]
        public int MainAccountId { get; set; }
    }
}
