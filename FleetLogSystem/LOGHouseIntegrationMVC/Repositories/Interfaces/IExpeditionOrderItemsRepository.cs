using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IExpeditionOrderItemsRepository
    {
        Task<ExpeditionOrderItem> Add(ExpeditionOrderItem expeditionOrderItem);
        ExpeditionOrder AddItem(ExpeditionOrderItemViewModel expeditionOrderItem);
        List<ExpeditionOrderItem> GetByOrderId(int id);

        Task<List<ExpeditionOrderItem>> GetByOrderIdAsync(int id);
        Task<ExpeditionOrderItem> UpdateAsync(ExpeditionOrderItem item);
        Task<ExpeditionOrderItem> GetByIdAsync(int id);

        Task<bool> DeleteByItemId(int itemId);
        Task<List<ExpeditionOrderItem>> GetAllByStatusOrdersIsNotAndMinimusDateAsync(List<ExpeditionOrderStatus> expeditionOrdersStatusUsable, DateTime baseDate, int productId, int clientId);
    }
}
