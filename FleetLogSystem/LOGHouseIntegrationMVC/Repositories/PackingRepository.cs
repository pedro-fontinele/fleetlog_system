using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using PagedList;

namespace LOGHouseSystem.Repositories
{
    public class PackingRepository : RepositoryBase, IPackingRepository
    {
        public Packing GetById(int id)
        {
            return _db.Packings
                   .Include(x => x.ExpeditionOrder)
                   .FirstOrDefault(x => x.Id == id);
        }

        public List<Packing> GetWithStatusGeradoOrEmAtendimento()
        {
            return _db.Packings
                 .AsNoTracking()
                 .Where(x => x.Status == Infra.Enums.PackingStatus.Gerado || x.Status == Infra.Enums.PackingStatus.EmAtendimento)
                 .Include(x => x.ExpeditionOrder)
                 .Include(x => x.Client)
                 .ToList();
        }

        public Packing Add(Packing packing)
        {
            _db.Packings.Add(packing);
            _db.SaveChanges();

            return packing;
        }



        public async Task<bool> Delete(Packing packing)
        {
            _db.Packings.Remove(packing);
           await _db.SaveChangesAsync();

            return true;
        }


        public async Task<List<Packing>> GetByStatusAsync(PackingStatus status)
        {
            return await _db.Packings
                .Where(x => x.Status == status)
                .AsNoTracking()
                .Include(x => x.Items)
                .Include(x => x.Client)
                .Include(x => x.ExpeditionOrder)
                .ToListAsync();
        }

        public async Task<Packing> UpdateStatusAsync(Packing packing, PackingStatus status)
        {
            packing.Status = PackingStatus.Cancelado;
            await UpdateAsync(packing);

            return packing;
        }

        public async Task<Packing> GetByIdAsync(int id)
        {
            return await _db.Packings
                .Include(x => x.ExpeditionOrder)
                .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Packing GetByExpeditionOrderId(int expeditionOrderId)
        {
            return _db.Packings
                .AsNoTracking()
                .Include(x => x.ExpeditionOrder)
                .Include(x => x.Items)
                .ThenInclude(x => x.Product)                
                .Include(e => e.PackingHistories)
                .ThenInclude(e => e.User)
                .Include(e => e.Client)
                .Include(e => e.PackingListTransportation)
                .ThenInclude(e => e.PackingListTransportationHistories)
                .FirstOrDefault(x => x.ExpeditionOrderId == expeditionOrderId);
        }

        public async Task<Packing> UpdateAsync(Packing packing)
        {
            var entry = _db.Entry(packing);

            if (entry.State == EntityState.Detached)
            {
                var set = _db.Set<PickingList>();
                PickingList attachedEntity = set.Find(packing.Id);  // You need to have access to key

                if (attachedEntity != null)
                {
                    var attachedEntry = _db.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(packing);
                }
                else
                {
                    entry.State = EntityState.Modified; // This should attach entity
                }
            }

            await _db.SaveChangesAsync();            

            return packing;
        }

        public PagedList<Packing> GetAllPaged(int page, int pageSize)
        {
            return (PagedList<Packing>)_db.Packings
                         .AsNoTracking()
                         .Include(x => x.ExpeditionOrder)
                         .Include(x => x.Client)
                         .OrderByDescending(x => x.Id)
                         .ToPagedList(page,pageSize);
        }

        public Task<Packing> GetByExpeditionOrderIdAsync(int id, bool takeExpeditionOrder = true)
        {
            var query = _db.Packings
                .AsNoTracking();

            if (takeExpeditionOrder)
            {
                query = query.Include(x => x.ExpeditionOrder);
            }
                
            query = query.Include(x => x.Items)
            .ThenInclude(x => x.Product);                

            return query.FirstOrDefaultAsync(x => x.ExpeditionOrderId == id);
        }


        public PagedList<Packing> GetPackingWhithoutPackingListTrasportationByFilter(FilterPackingWhithoutPackingListTrasportationViewModel filter)
        {

            int PageSize = 100;

            var query = _db.Packings
               .Include(x => x.ExpeditionOrder)
               .Include(x => x.Client)
               .AsQueryable();

            query = query.Where(x => x.ExpeditionOrder.PackingListTransportationId == null);

            if (filter.CreatedAtStart != null)
            {
                DateTime date = filter.CreatedAtStart.Value.Date;
                query = query.Where(x => x.CreatedAt.Date >= date.Date);
            }

            if (filter.CreatedAtEnd != null)
            {
                DateTime date = filter.CreatedAtEnd.Value.Date;
                query = query.Where(x => x.CreatedAt.Date <= date.Date);
            }

            query = query.OrderByDescending(x => x.Id);

            return (PagedList<Packing>)query
                .AsNoTracking()
                .OrderBy(e => e.CreatedAt)
                .ToPagedList(filter.Page, PageSize);
        }

        public PagedList<Packing> GetByFilters(FilterPackingViewModel filter)
        {
            int PageSize = 50;

            var query = GetQueryableFilter(filter);

            return (PagedList<Packing>)query
                .AsNoTracking()
                .ToPagedList(filter.Page, PageSize);
        }

        public IQueryable<Packing> GetQueryableFilter(FilterPackingViewModel filter)
        {
            var query = _db.Packings
                .Include(x => x.ExpeditionOrder)
                .ThenInclude(x => x.ExpeditionOrderItems)
                .Include(x => x.Client)
                .AsQueryable();

            if (filter.InvoiceNumber != null)
                query = query.Where(x => x.ExpeditionOrder.InvoiceNumber == filter.InvoiceNumber);

            if (filter.PickingListId != null)
                query = query.Where(x => x.ExpeditionOrder.PickingListId == filter.PickingListId);


            if (filter.ClientId != null)
                query = query.Where(x => x.ClientId == filter.ClientId);



            if (filter.CreatedAt != null)
            {
                DateTime date = filter.CreatedAt.Value.Date;
                query = query.Where(x => x.CreatedAt.Date == date.Date);
            }

            if (filter.Status != null)
                query = query.Where(x => x.Status == filter.Status);

            query = query.OrderByDescending(x => x.Id);

            return query;

        }

       
    }
}
