using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IInventoryMovementRepository
    {
        InventoryMovement AddInventoryMovement(InventoryMovement movement);
        Task DeleteAllByProductId(int productId);
        Task DeleteAllbyProductIdAndDate(int productId, DateTime dateTime);
        Task DisableAllByProductAndDate(int productId, DateTime dateTime);
        InventoryMovement MappingEntryMovement(InventoryType type, double quantity, double quantityStockFinal, int productId, StockSlotMovimentEnum stockSlot, OriginInventoryMovimentEnum originInventoryMoviment, DateTime? date, string? note = null);
    }
}
