namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Response
{
    public class GetTrackingNumberResponseShopeeDto
    {
        public string error { get; set; }
        public string message { get; set; }
        public GetTrackingNumberDetailsResponseShopeeDto response { get; set; }
        public string request_id { get; set; }
    }

    public class GetTrackingNumberDetailsResponseShopeeDto
    {
        public string tracking_number { get; set; }
        public string first_mile_tracking_number { get; set; }
    }
}
