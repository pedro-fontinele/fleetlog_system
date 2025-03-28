using System.Collections.Generic;

namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Response
{
    public class GetShippingParamtersShopeeResponse
    {
        public string error { get; set; }
        public string message { get; set; }
        public GetShippingParamtersShopeeResponseBody response { get; set; }
        public string request_id { get; set; }
    }

    public class GetShippingParamtersShopeeResponseBody
    {
        public GetShippingParamtersShopeeInfoNeeded info_needed { get; set; }
        public GetShippingParamtersShopeeDropoff dropoff { get; set; }
        public GetShippingParamtersShopeePickup pickup { get; set; }
    }

    public class GetShippingParamtersShopeeInfoNeeded
    {
        public List<string> dropoff { get; set; }
        public List<string> pickup { get; set; }
    }

    public class GetShippingParamtersShopeeDropoff
    {
        public List<GetShippingParamtersShopeeBranchList> branch_list { get; set; }
        public List<GetShippingParamtersShopeeSlugList> slug_list { get; set; }
    }

    public class GetShippingParamtersShopeePickup
    {
        public List<GetShippingParamtersShopeeAddressList> address_list { get; set; }
        public List<GetShippingParamtersShopeeTimeSlotList> time_slot_list { get; set; }
    }

    public class GetShippingParamtersShopeeBranchList
    {
        public int branch_id { get; set; }        
    }

    public class GetShippingParamtersShopeeSlugList
    {
        public string slug { get; set; }        
    }

    public class GetShippingParamtersShopeeAddressList
    {
        public string address_id { get; set; }        
    }

    public class GetShippingParamtersShopeeTimeSlotList
    {
        public string pickup_time_id { get; set; }        
    }
}
