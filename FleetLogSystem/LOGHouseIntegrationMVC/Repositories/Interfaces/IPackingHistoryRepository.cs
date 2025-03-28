using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IPackingHistoryRepository
    {
        Task<PackingHistory> Add(PackingHistory history);

        PackingHistory AddNotAsync(PackingHistory history);

        Task<List<PackingHistory>> GetAllByPackingId(int id);
    }
}
