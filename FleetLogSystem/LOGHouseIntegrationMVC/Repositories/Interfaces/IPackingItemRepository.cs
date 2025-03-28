using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IPackingItemRepository
    {
        List<PackingItem> GetAllByPackingId(int id);

        PackingItem Add(PackingItem packingItem);

        IQueryable<PackingItem> GetAll();
        Task<List<PackingItem>> UpdatePackingItemByStatusAsync(int packingId, PackingItemStatus status);


        PackingItem Update(PackingItem packingItem);
    }
}
