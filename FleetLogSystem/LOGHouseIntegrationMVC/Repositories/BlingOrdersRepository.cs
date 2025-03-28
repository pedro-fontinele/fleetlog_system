using LOGHouseSystem.Models;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Helpers;
using Microsoft.EntityFrameworkCore;
using LOGHouseSystem.Repositories.Interfaces;

namespace LOGHouseSystem.Repositories
{
    public class BlingOrdersRepository : RepositoryBase, IBlingOrdersRepository
    {
        public BlingOrdersRepository(AppDbContext appDb) : base(appDb)
        {

        }

        public async Task<BlingOrder> Add(BlingOrder order)
        {
            order.EntryDate = DateTime.Now;
            _db.BlingOrders.Add(order);
            await _db.SaveChangesAsync();

            return order;
        }
    }
}
