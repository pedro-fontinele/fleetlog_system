using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IPackingHistoryService
    {
        Task Add(int packingId, string obs, PackingStatus status, int userId = 0);
    }
}
