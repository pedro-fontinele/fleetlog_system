using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;

namespace LOGHouseSystem.Repositories
{
    public class TinyOrdersRepository : RepositoryBase, ITinyOrdersRepository
    {
        public TinyOrdersRepository(AppDbContext appDb) : base(appDb)
        {

        }

        public async Task<TinyOrder> Add(TinyOrder tinyOrder)
        {
            tinyOrder.EntryDate = DateTime.Now;
            _db.TinyOrder.Add(tinyOrder);
            await _db.SaveChangesAsync();
            return tinyOrder;
        }
    }
}
