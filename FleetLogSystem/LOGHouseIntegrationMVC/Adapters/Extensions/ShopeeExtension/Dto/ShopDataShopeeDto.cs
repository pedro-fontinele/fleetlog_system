using LOGHouseSystem.Adapters.Extensions.BaseOAUTH2Extension.Dto;

namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto
{
    public class ShopDataShopeeDto : AuthenticationBaseDto
    {
        public int ShopId { get; set; }
        public int MerchantId { get; set; }
        public int MainAccountId { get; internal set; }
    }
}
