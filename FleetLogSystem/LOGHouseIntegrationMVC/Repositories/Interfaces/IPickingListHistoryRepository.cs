using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IPickingListHistoryRepository
    {
        Task<PickingListHistory> Add(PickingListHistory history);
        Task<List<PickingListHistory>> GetByPickingId(int pickingId);
    }
}
