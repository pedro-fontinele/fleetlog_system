using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class PackingItemRepository : RepositoryBase, IPackingItemRepository
    {
        public List<PackingItem> GetAllByPackingId(int id)
        {
            return _db.PackingItems
                      .AsNoTracking()
                      .Where(x => x.PackingId == id)
                      .Include(x => x.Product)
                      .ToList();
        }

        public async Task<List<PackingItem>> UpdatePackingItemByStatusAsync(int packingId, PackingItemStatus status)
        {
            List<PackingItem> items = await _db.PackingItems.Where(x => x.PackingId == packingId).ToListAsync();

            foreach (PackingItem item in items)
            {
                item.Status = status;
                _db.Update(item);
            }

            await _db.SaveChangesAsync();
            return items;
        }

        public PackingItem Add(PackingItem packing)
        {
            _db.PackingItems.Add(packing);
            _db.SaveChanges();

            return packing;
        }

        public IQueryable<PackingItem> GetAll()
        {
            var query = _db.PackingItems
                .AsNoTracking()
                .Include(x => x.Product)
                .Select(packingListItem => packingListItem);
            return query;
        }

        public PackingItem Update(PackingItem packingItem)
        {
            var entry = _db.Entry(packingItem);

            if (entry.State == EntityState.Detached)
            {
                var set = _db.Set<PackingItem>();
                PackingItem attachedEntity = set.Find(packingItem.Id);  // You need to have access to key

                if (attachedEntity != null)
                {
                    var attachedEntry = _db.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(packingItem);
                }
                else
                {
                    entry.State = EntityState.Modified; // This should attach entity
                }
            }

            _db.SaveChanges();
            return packingItem;
        }
    }
}
