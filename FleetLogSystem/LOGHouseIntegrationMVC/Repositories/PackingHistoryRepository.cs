using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class PackingHistoryRepository : RepositoryBase, IPackingHistoryRepository
    {
        public async Task<PackingHistory> Add(PackingHistory history)
        {
            if (history == null)
                throw new Exception("Não é possível adicionar um histórico nulo");

            _db.PackingHistories.Add(history);
            await _db.SaveChangesAsync();

            return history;
        }

        public PackingHistory AddNotAsync(PackingHistory history)
        {
            if (history == null)
                throw new Exception("Não é possível adicionar um histórico nulo");

            _db.PackingHistories.Add(history);
            _db.SaveChanges();

            return history;
        }

        public async Task<List<PackingHistory>> GetAllByPackingId(int id)
        {
            if (id == 0)
                throw new Exception("Não foi possível encontrar o histórico na lista de sepração indicada");

            return await _db.PackingHistories.Where(x => x.PackingId == id).Include(x => x.User).ToListAsync();
        }
    }
}
