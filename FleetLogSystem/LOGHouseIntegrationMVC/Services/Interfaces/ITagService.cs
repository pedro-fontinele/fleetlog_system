using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface ITagService
    {
        public Task ProcessMarketplaceTag(ExpeditionOrder order);
        
    }
}
