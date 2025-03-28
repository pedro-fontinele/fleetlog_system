using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using PagedList;

namespace LOGHouseSystem.Repositories
{
    public class PackingListTransportationRepository : RepositoryBase, IPackingListTransportationRepository
    {
        public async Task<PackingListTransportation> AddAsync(PackingListTransportation request)
        {
           await _db.PackingListTransportations.AddAsync(request);
           await  _db.SaveChangesAsync();

            return request;

        }

        public List<PackingListTransportation> GetByStatus(PackingListTransportationStatus status)
        {
            return _db.PackingListTransportations
                .Where(x => x.Status == status)
                .AsNoTracking()
                .Include(x => x.ShippingCompany)
                .Include(x => x.TransportationPerson)
                .ToList();
        }

        public PagedList<PackingListTransportation> GetAllPaged(int page, int pageSize)
        {
            var query = _db.PackingListTransportations.Include(p => p.ShippingCompany).Include(p => p.TransportationPerson).Include(p => p.ExpeditionOrders);

            return (PagedList<PackingListTransportation>)query
                      .OrderByDescending(x => x.Id)
                      .ToPagedList(page, pageSize);
        }

        public PackingListTransportation? GetById(int id)
        {
            return _db.PackingListTransportations
                .Include(p => p.ShippingCompany)
                .Include(p => p.TransportationPerson)
                .Include(p => p.ExpeditionOrders)
                .FirstOrDefault(m => m.Id == id);
        }

        public async Task<PackingListTransportation> UpdateAsync(PackingListTransportation packingListTransportation)
        {
            _db.Entry(packingListTransportation).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return packingListTransportation;
        }

        public object GetIfContainsPrefix(string prefix)
        {
            var list = (from lists in _db.ShippingCompanies
                           where lists.Name.Contains(prefix)
                           select new
                           {
                               label = lists.Name,
                               val = lists.Id
                           }).ToList();

            return (list);
        }

        public PagedList<PackingListTransportation> GetByFilters(FilterPackingListTransportationViewModel filter)
        {
            int PageSize = 50;

            var query = GetQueryableFilter(filter);

            return (PagedList<PackingListTransportation>)query
                .AsNoTracking()
                //.Include(x => x.ReceiptNoteItems)
                .ToPagedList(filter.Page, PageSize);
        }

        public IQueryable<PackingListTransportation> GetQueryableFilter(FilterPackingListTransportationViewModel filter)
        {
            var query = _db.PackingListTransportations
                .Include(x => x.ExpeditionOrders)
                .Include(x => x.ShippingCompany)
                .AsQueryable();

            if (filter.InvoiceNumber != null)
                query = query.Where(x => x.ExpeditionOrders.Any(order => order.InvoiceNumber == filter.InvoiceNumber));


            if (filter.ShippingCompanyId != null)
                query = query.Where(x => x.ShippingCompany.Id == filter.ShippingCompanyId);


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
