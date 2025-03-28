using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IExpeditionOrderHistoryRepository
    {
        ExpeditionOrderHistory AddNotAsync(ExpeditionOrderHistory history);
        Task<ExpeditionOrderHistory> Add(ExpeditionOrderHistory history);
        Task<List<ExpeditionOrderHistory>> GetByOrderIdAndStatusAsync(int id, ExpeditionOrderStatus status);
        Task<List<ExpeditionOrderHistory>> GetByOrderIdAsync(int orderId);
    }
}
