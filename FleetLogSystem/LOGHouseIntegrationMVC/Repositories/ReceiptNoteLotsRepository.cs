using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class ReceiptNoteLotsRepository : RepositoryBase, IReceiptNoteLotsRepository
    {
        public List<ReceiptNoteLots> GetByProductId(int productId)
        {
            return _db.ReceiptNoteLots
                      .AsNoTracking()
                      .Where(x => x.ProductId == productId)
                      .ToList();
        }

        public ReceiptNoteLots GetLotById(int id)
        {
            return _db.ReceiptNoteLots.Find(id);
        }

        public async Task<ReceiptNoteLots> GetLotByIdAsync(int id)
        {
            return await _db.ReceiptNoteLots.FindAsync(id);
        }

        public ReceiptNoteLots Add(ReceiptNoteLots lot)
        {
            _db.ReceiptNoteLots.Add(lot);
            _db.SaveChanges();

            return lot;
        }

        public IQueryable<ReceiptNoteLots> GetAll()
        {
            var query = _db.ReceiptNoteLots
                .AsNoTracking()
                .Select(receiptNoteLot => receiptNoteLot);
            return query;
        }

        public ReceiptNoteLots Update(ReceiptNoteLots lot)
        {
            _db.Entry(lot).State = EntityState.Modified;
            _db.SaveChanges();
            return lot;
        }

        public List<ReceiptNoteLots> GetByProductIdAndStatus(int productId, LotStatus status)
        {
            return _db.ReceiptNoteLots
                      .AsNoTracking()
                      .Where(x => x.ProductId == productId && x.Status == status)
                      .OrderBy(x=>x.Id)
                      .ToList();
        }

        public async Task<ReceiptNoteLots> UpdateAsync(ReceiptNoteLots lot)
        {
            _db.Entry(lot).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return lot;
        }

        public async Task<List<ReceiptNoteLots>> GetByProductIdAndStatusAsync(int productId, LotStatus status)
        {
            return await _db.ReceiptNoteLots
                      .AsNoTracking()
                      .Where(x => x.ProductId == productId && x.Status == status)
                      .OrderBy(x => x.Id)
                      .ToListAsync();
        }

        public async Task<PaginationBase<ReceiptNoteLots>> GetAllAsync(ReceiptNoteIndexLotsPaginationRequest request)
        {
            var query = _db.ReceiptNoteLots.Include(e => e.Product).OrderBy(e => e.Id).AsQueryable();

            if (request.ProductId > 0)
            {
                query = query.Where(e => e.ProductId == request.ProductId);
            }
            return await PaginateQueryWithRequest(query, request);
        }
    }
}
