using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IPackingListTransportationHistoryRepository
    {
        PackingListTransportationHistory Add(PackingListTransportationHistory item);
        Task<PackingListTransportationHistory> AddAsync(PackingListTransportationHistory item);
    }
}
