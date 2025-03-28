using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;

namespace LOGHouseSystem.Repositories
{
    public class PackingListTransportationHistoryRepository : RepositoryBase, IPackingListTransportationHistoryRepository
    {
        public PackingListTransportationHistory Add(PackingListTransportationHistory item)
        {
            _db.PackingListTransportationHistories.Add(item);
            _db.SaveChanges();

            return item;
        }

        public async Task<PackingListTransportationHistory> AddAsync(PackingListTransportationHistory item)
        {
            _db.PackingListTransportationHistories.Add(item);
            _db.SaveChangesAsync();

            return item;
        }
    }
}
