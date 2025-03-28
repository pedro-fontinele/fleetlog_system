using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IPickingListHistoryService
    {
        Task Add(int pickingId, string obs, PickingListStatus status, int userId = 0);

        Task<List<PickingListHistory>> GetByPickingId(int pickingId);
    }
}
