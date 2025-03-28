using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IExpeditionOrderHistoryService
    {
        Task Add(int? orderId, string obs, ExpeditionOrderStatus status, int userId = 0);

        void AddNotAsync(int? orderId, string obs, ExpeditionOrderStatus status, int userId = 0);

        Task<List<ExpeditionOrderHistory>> GetByOrderId(int Id);
    }
}
