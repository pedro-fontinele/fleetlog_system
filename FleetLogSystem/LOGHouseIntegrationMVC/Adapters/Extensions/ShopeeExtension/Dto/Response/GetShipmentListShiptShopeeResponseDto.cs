using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Response
{
    public class GetShipmentListShiptShopeeResponseDto
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        public ResponseGetShipmentListShiptShopeeResponseDto Response { get; set; }
    }

    public class ResponseGetShipmentListShiptShopeeResponseDto
    {
        [JsonProperty("order_list")]
        public List<ShipmentListShiptShopeeDto> OrderList { get; set; }

        [JsonProperty("next_cursor")]
        public int NextCursor { get; set; }

        [JsonProperty("more")]
        public bool More { get; set; }

        [JsonProperty("request_id")]
        public string RequestId { get; set; }
    }
}
