using LOGHouseSystem.Adapters.BaseOAUTH2Extension.Dto;
using LOGHouseSystem.Adapters.Extensions.BaseOAUTH2Extension.Interface;
using LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto;

namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension {
    /// <summary>
    /// Needs to be implemented by the Extension user system.
    /// This class is used by the user system to implement their database rule with Shopee integration
    /// </summary>
    public interface IDataShopeeService : IDataAuthenticationService
    {
        /// <summary>
        /// Get Shopee data partner 
        /// </summary>
        /// <returns></returns>
        Task<ShopDataShopeeDto> GetShopData(int clientId);
    }
}
