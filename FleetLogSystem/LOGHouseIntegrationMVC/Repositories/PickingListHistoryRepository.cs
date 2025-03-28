using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class PickingListHistoryRepository : RepositoryBase, IPickingListHistoryRepository
    {
        public async Task<PickingListHistory> Add(PickingListHistory history)
        {
            if (history == null)
                throw new Exception("Não é possível adicionar um histórico nulo");

            _db.PickingListHistories.Add(history);
            await _db.SaveChangesAsync();

            return history;
        }

        public async Task<List<PickingListHistory>> GetByPickingId(int pickingId)
        {
            if (pickingId == 0)
                throw new Exception("Não foi possível encontrar o histórico dessa lista de sepração");

            return await _db.PickingListHistories.Where(x => x.PickingListId == pickingId).Include(x => x.User).ToListAsync();
        }
    }
}
