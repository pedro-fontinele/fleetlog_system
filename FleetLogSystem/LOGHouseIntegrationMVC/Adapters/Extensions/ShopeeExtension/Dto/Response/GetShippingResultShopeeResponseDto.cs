namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Response
{
    public class GetShippingResultShopeeResponseDto
    {
        public string error { get; set; }
        public string message { get; set; }
        public GetShippingResultResponseShopeeDto response { get; set; }
        public object warning { get; set; }
        public string request_id { get; set; }
    }

    public class GetShippingResultResponseShopeeDto
    {
        public List<GetShippingResultResultShopeeDto> result_list { get; set; }
    }

    public class GetShippingResultResultShopeeDto
    {
        public string order_sn { get; set; }
        public string status { get; set; }
    }
}
