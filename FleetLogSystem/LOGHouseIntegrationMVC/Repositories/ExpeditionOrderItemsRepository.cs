using DocumentFormat.OpenXml.Office2010.Excel;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace LOGHouseSystem.Repositories
{
    public class ExpeditionOrderItemsRepository : RepositoryBase, IExpeditionOrderItemsRepository
    {


        private readonly IExpeditionOrderRepository _expeditionOrderRepository;
        public ExpeditionOrderItemsRepository(AppDbContext appDb, IExpeditionOrderRepository expeditionOrderRepository) : base(appDb)
        {
            _expeditionOrderRepository = expeditionOrderRepository;
        }

        public async Task<ExpeditionOrderItem> Add(ExpeditionOrderItem expeditionOrderItem)
        {
            _db.ExpeditionOrderItems.Add(expeditionOrderItem);
            await _db.SaveChangesAsync();

            return expeditionOrderItem;
        }

        public ExpeditionOrder AddItem(ExpeditionOrderItemViewModel item)
        {
            ExpeditionOrderItem newItem = new ExpeditionOrderItem()
            {
                Name = item.Name,
                Ean = item.Ean,
                Description = item.Description,
                Quantity = item.Quantity,
                ProductId = item.ProductId,
                ExpeditionOrderId = item.ExpeditionOrderId
            };

            _db.ExpeditionOrderItems.Add(newItem);
            _db.SaveChanges();

            ExpeditionOrder expeditionOrder = _expeditionOrderRepository.GetById(item.Id);

            return expeditionOrder;
        }

        public async Task<ExpeditionOrderItem> GetByIdAsync(int itemId)
        {
            return await _db.ExpeditionOrderItems.FirstOrDefaultAsync(x => x.Id == itemId);
        }

        public async Task<bool> DeleteByItemId(int itemId)
        {
            ExpeditionOrderItem item = await GetByIdAsync(itemId);

            if (item != null)
            {
                _db.ExpeditionOrderItems.Remove(item);
                await _db.SaveChangesAsync();
                return true;
            }

            return false; 
        }


        public List<ExpeditionOrderItem> GetByOrderId(int id)
        {
            return _db.ExpeditionOrderItems
                      .Where(x => x.ExpeditionOrderId == id)
                      .ToList();
        }

        public async Task<List<ExpeditionOrderItem>> GetByOrderIdAsync(int id)
        {
                return await _db.ExpeditionOrderItems
                          .Where(x => x.ExpeditionOrderId == id)
                          .ToListAsync();
        }

        public async Task<ExpeditionOrderItem> UpdateAsync(ExpeditionOrderItem item)
        {
            var entry = _db.Entry(item);

            if (entry.State == EntityState.Detached)
            {
                var set = _db.Set<ExpeditionOrderItem>();
                ExpeditionOrderItem attachedEntity = set.Find(item.Id);  // You need to have access to key

                if (attachedEntity != null)
                {
                    var attachedEntry = _db.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(item);
                }
                else
                {
                    entry.State = EntityState.Modified; // This should attach entity
                }
            }

            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<List<ExpeditionOrderItem>> GetAllByStatusOrdersIsNotAndMinimusDateAsync(List<ExpeditionOrderStatus> status, DateTime baseDate, int productId, int clientId)
        {
            List<int> enumIntValues = status.Select(a => (int)a).ToList();

            return await _db.ExpeditionOrderItems.Include(e => e.ExpeditionOrder)
            .Where(x => enumIntValues.Contains((int)x.ExpeditionOrder.Status) && (x.ExpeditionOrder.IssueDate >= baseDate || x.ExpeditionOrder.CreationDate >= baseDate) && x.ProductId == productId && x.ExpeditionOrder.ClientId == clientId)
            .ToListAsync();
        }
    }
}
