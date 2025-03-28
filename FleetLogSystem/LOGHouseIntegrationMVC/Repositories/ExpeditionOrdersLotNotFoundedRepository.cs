using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class ExpeditionOrdersLotNotFoundedRepository : RepositoryBase, IExpeditionOrdersLotNotFoundedRepository
    {
        public async Task<ExpeditionOrdersLotNotFounded> AddAsync(ExpeditionOrdersLotNotFounded order)
        {
            order.EntryDate = DateTime.Now;
            _db.ExpeditionOrdersLotNotFounded.Add(order);
            await _db.SaveChangesAsync();
            return order;
        }

        public async Task<List<ExpeditionOrdersLotNotFounded>> GetAllByProductIdAndStatus(int productId, ExpeditionOrdersLotNotFoundedStatusEnum status)
        {
            var data = await _db.ExpeditionOrdersLotNotFounded.Where(e => e.ProductId == productId && e.Status == status).ToListAsync();

            return data;
        }

        public async Task<PaginationBase<ExpeditionOrdersLotNotFounded>> GetAllNotFoundedOrders(NotFoundedPaginationRequestViewModel pagination)
        {
            var query = _db.ExpeditionOrdersLotNotFounded.Include(e => e.Product).Include(e => e.ExpeditionOrder).AsQueryable();

            if (pagination.Status != null)
            {
                query = query.Where(e => e.Status == pagination.Status);
            }

            return await PaginateQueryWithRequest<ExpeditionOrdersLotNotFounded>(query, pagination);
        }

        public async Task<List<ExpeditionOrdersLotNotFounded>> GetByClientIdAndStatus(int clientId, ExpeditionOrdersLotNotFoundedStatusEnum status)
        {
            var data = await _db.ExpeditionOrdersLotNotFounded.Include(e => e.ExpeditionOrder).ThenInclude(e => e.ExpeditionOrderItems).Where(e => e.ExpeditionOrder.ClientId == clientId && e.Status == status).ToListAsync();

            return data;
        }

        public async Task<List<ExpeditionOrdersLotNotFounded>> GetByExpeditionOrderIds(List<int> expeditionOrderIds)
        {
            var data = await _db.ExpeditionOrdersLotNotFounded.Include(e => e.ExpeditionOrder).ThenInclude(e => e.ExpeditionOrderItems).Where(e => expeditionOrderIds.Any(f => f == e.ExpeditionOrderId)).ToListAsync();

            return data;
        }

        public async Task<List<ExpeditionOrdersLotNotFounded>> GetByExpeditionOrderIdsAndStatus(List<int> expeditionOrderIds, ExpeditionOrdersLotNotFoundedStatusEnum created)
        {
            var data = await _db.ExpeditionOrdersLotNotFounded.Include(e => e.ExpeditionOrder).ThenInclude(e => e.ExpeditionOrderItems).Where(e => expeditionOrderIds.Any(f => f == e.ExpeditionOrderId) && e.Status == created).ToListAsync();

            return data;
        }

        public async Task<ExpeditionOrdersLotNotFounded> GetByIdAsync(int id)
        {
            var data = await _db.ExpeditionOrdersLotNotFounded.Include(e => e.ExpeditionOrder).ThenInclude(e => e.ExpeditionOrderItems).Where(e => e.Id == id).FirstOrDefaultAsync();

            return data;
        }

        public async Task UpdateAsync(ExpeditionOrdersLotNotFounded item)
        {
            _db.ExpeditionOrdersLotNotFounded.Update(item);
            await _db.SaveChangesAsync();
        }
    }
}
