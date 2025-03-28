namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Request
{
    public class ShipOrderShopeeRequest
    {
        public string order_sn { get; set; }
        public ShipOrderDropoffShopeeRequest dropoff { get; set; }
        public ShipOrderPickupShopeeRequest pickup { get; set; }
    }

    public class ShipOrderDropoffShopeeRequest
    {
        public int branch_id { get; set; }
        public string sender_real_name { get; set; }
        public string tracking_number { get; set; }
        public string slug { get; set; }
    }

    public class ShipOrderPickupShopeeRequest
    {
        public string address_id { get; set; }
        public string pickup_time_id { get; set; }
        public string tracking_number { get; set; }
    }
}
