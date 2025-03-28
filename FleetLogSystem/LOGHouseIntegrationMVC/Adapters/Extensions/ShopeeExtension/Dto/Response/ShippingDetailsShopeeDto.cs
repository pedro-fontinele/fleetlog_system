namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Response
{
    public class ShippingDetailsShopeeDto
    {
        public string error { get; set; }
        public string message { get; set; }
        public ShippingDetailsResponseShopeeDto response { get; set; }
        public string request_id { get; set; }
    }

    public class ShippingDetailsResponseShopeeDto
    {
        public List<ShippingDetailsOrderShopeeDto> order_list { get; set; }
    }

    public class ShippingDetailsOrderShopeeDto
    {
        public string order_sn { get; set; }
        public string region { get; set; }
        public string currency { get; set; }
        public bool cod { get; set; }
        public string order_status { get; set; }
        public string message_to_seller { get; set; }
        public long create_time { get; set; }
        public long update_time { get; set; }
        public int days_to_ship { get; set; }
        public long ship_by_date { get; set; }
        public int reverse_shipping_fee { get; set; }
    }
}
