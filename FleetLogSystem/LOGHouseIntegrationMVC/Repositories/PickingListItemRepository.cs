using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class PickingListItemRepository : RepositoryBase, IPickingListItemRepository
    {
        public PickingListItemRepository(AppDbContext db) : base(db)
        {

        }

        public async Task<PickingListItem> Create(PickingListItem picking)
        {
            _db.PickingListItems.Add(picking);
            await _db.SaveChangesAsync();
            return picking;
        }

        public async Task<bool> CancelByPickingListIdAsync(int id)
        {
            List< PickingListItem> items = await _db.PickingListItems.Where(x => x.PickingListId == id).ToListAsync();

            if(items.Count > 0)
            {
                foreach (var item in items)
                {
                    item.ItemStatus = Infra.Enums.PickingListItemStatus.Cancelado;

                    await UpdateAsync(item);
                }

                return true;
            }

            return false;

        }

        public IQueryable<PickingListItem> GetAll()
        {
            var query = _db.PickingListItems
                .AsNoTracking()
                .Include(x => x.Product)
                .Select(pickingListItem => pickingListItem);
            return query;
        }

        public List<PickingListItem> GetByPickingListId(int id)
        {
            var query = _db.PickingListItems
                .Where(x => x.PickingListId == id)
                .AsNoTracking()
                .Include(x => x.Product)
                .ToList();

            return query;
        }

        public async Task<PickingListItem> GetByProductIdAsync(int? productId, int pickingListId)
        {
            var item = await _db.PickingListItems
                .Where(x => x.ProductId == productId && x.PickingListId == pickingListId)
                .FirstOrDefaultAsync();

            return item;

        }

        public PickingListItem Update(PickingListItem pickingListItem)
        {
            var entry = _db.Entry(pickingListItem);

            if (entry.State == EntityState.Detached)
            {
                var set = _db.Set<PickingListItem>();
                PickingListItem attachedEntity = set.Find(pickingListItem.Id);  // You need to have access to key

                if (attachedEntity != null)
                {
                    var attachedEntry = _db.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(pickingListItem);
                }
                else
                {
                    entry.State = EntityState.Modified; // This should attach entity
                }
            }

            _db.SaveChanges();
            return pickingListItem;
        }

        public async Task<PickingListItem> UpdateAsync(PickingListItem pickingListItem)
        {
            var entry = _db.Entry(pickingListItem);

            if (entry.State == EntityState.Detached)
            {
                var set = _db.Set<PickingListItem>();
                PickingListItem attachedEntity = set.Find(pickingListItem.Id);  // You need to have access to key

                if (attachedEntity != null)
                {
                    var attachedEntry = _db.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(pickingListItem);
                }
                else
                {
                    entry.State = EntityState.Modified; // This should attach entity
                }
            }

            await _db.SaveChangesAsync();
            return pickingListItem;
        }

    }
}
