using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Request
{
    public class ErrorShopeeDto
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        [JsonProperty("response")]
        public ErrorResponseResultListShopeeDto Response { get; set; }
    }

    public class ErrorResponseResultListShopeeDto
    {
        [JsonProperty("result_list")]
        public List<ErrorResultListShopeeDto> ResultList { get; set; }
    }

    public class ErrorResultListShopeeDto
    {
        [JsonProperty("order_sn")]
        public string Order { get; set; }

        [JsonProperty("fail_error")]
        public string FailError { get; set; }

        [JsonProperty("fail_message")]
        public string FailMessage { get; set; }

    }
}
