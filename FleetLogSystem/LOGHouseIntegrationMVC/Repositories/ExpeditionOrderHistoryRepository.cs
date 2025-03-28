using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class ExpeditionOrderHistoryRepository : RepositoryBase, IExpeditionOrderHistoryRepository
    {
        public async Task<ExpeditionOrderHistory> Add(ExpeditionOrderHistory history)
        {
            if (history == null)
                throw new Exception("Não é possível adicionar um histórico nulo");

            _db.ExpeditionOrderHistories.Add(history);
            await _db.SaveChangesAsync();

            return history;
        }

        public async Task<List<ExpeditionOrderHistory>> GetByOrderIdAndStatusAsync(int id, ExpeditionOrderStatus status)
        {
            return await _db.ExpeditionOrderHistories.Include(x => x.User).Where(x => x.ExpeditionOrderId == id && x.Status == status).OrderBy(x => x.Date).ToListAsync();
        }

        public async Task<List<ExpeditionOrderHistory>> GetByOrderIdAsync(int orderId)
        {
            return await _db.ExpeditionOrderHistories.Include(x => x.User).Where(x => x.ExpeditionOrderId == orderId).OrderBy(x => x.Date).ToListAsync();
        }

        public ExpeditionOrderHistory AddNotAsync(ExpeditionOrderHistory history)
        {
            if (history == null)
                throw new Exception("Não é possível adicionar um histórico nulo");

            _db.ExpeditionOrderHistories.Add(history);
            _db.SaveChanges();

            return history;
        }
    }
}
