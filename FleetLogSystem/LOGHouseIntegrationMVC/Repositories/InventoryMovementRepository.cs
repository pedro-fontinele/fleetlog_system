using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class InventoryMovementRepository : RepositoryBase, IInventoryMovementRepository
    {
        public InventoryMovement AddInventoryMovement(InventoryMovement movement)
        {
            _db.InventoryMovements.Add(movement);
            _db.SaveChanges();

            return movement;
        }

        public async Task DeleteAllByProductId(int productId)
        {
            var products = await _db.InventoryMovements.Where(e => e.ProductId == productId).ToListAsync();

            _db.InventoryMovements.RemoveRange(products);

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAllbyProductIdAndDate(int productId, DateTime dateTime)
        {
            var products = await _db.InventoryMovements.Where(e => e.ProductId == productId && e.Date >= dateTime).ToListAsync();

            _db.InventoryMovements.RemoveRange(products);

            await _db.SaveChangesAsync();
        }

        public async Task DisableAllByProductAndDate(int productId, DateTime dateTime)
        {
            var products = await _db.InventoryMovements.Where(e => e.ProductId == productId && e.Date >= dateTime).ToListAsync();

            products = products.Select(e =>
            {
                e.Status = false;
                return e;

            }).ToList();

            _db.InventoryMovements.UpdateRange(products);

            await _db.SaveChangesAsync();
        }

        public InventoryMovement MappingEntryMovement(InventoryType type, double quantity, double quantityStockFinal, int productId, StockSlotMovimentEnum stockSlot, OriginInventoryMovimentEnum originInventoryMoviment, DateTime? date, string? note = null)
        {
            InventoryMovement invMov = new InventoryMovement()
            {
                Type = type,
                Quantity = quantity,
                QuantityFinalQuantity = quantityStockFinal,
                ProductId = productId,
                Note = note,
                StockSlotMoviment = stockSlot,
                Origin = originInventoryMoviment,
                Date = date ?? DateTime.Now,
                Status = true
            };

           return AddInventoryMovement(invMov);
        }
    }
}
