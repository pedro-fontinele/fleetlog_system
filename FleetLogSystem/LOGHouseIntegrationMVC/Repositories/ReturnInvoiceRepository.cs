using DocumentFormat.OpenXml.Office2010.Excel;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class ReturnInvoiceRepository : RepositoryBase, IReturnInvoiceRepository
    {
        public async Task<ReturnInvoice> Add(ReturnInvoice invoice)
        {
            _db.ReturnInvoices.Add(invoice);
            await _db.SaveChangesAsync();

            return invoice;
        }

        public async Task<List<ReturnInvoice>> GetAllById(List<int> returnInvoices)
        {
            return await _db.ReturnInvoices.Where(e => returnInvoices.Any(a => a == e.Id)).ToListAsync();
        }

        public async Task<ReturnInvoice> GetByExternalId(string externalId)
        {
            return await _db.ReturnInvoices.Where(e => e.ExternalId == externalId).FirstOrDefaultAsync();
        }

        public async Task<ReturnInvoice> GetByIdAsync(int Id)
        {
            return await _db.ReturnInvoices.Include(e => e.ReturnInvoiceItems).ThenInclude(e => e.Product).Include(e => e.Client).Where(e => e.Id == Id).FirstOrDefaultAsync();
        }

        public async Task<PaginationBase<ReturnInvoice>> GetByPagination(PaginationRequest request)
        {
            var query = _db.ReturnInvoices.Include(x => x.Client).Include(e => e.ReturnInvoiceItems).OrderByDescending(e => e.CreatedAt).ThenByDescending(e => e.Id).AsQueryable();

            var response = await PaginateQueryWithRequest(query, request);

            response.Data = response.Data.OrderByDescending(e =>e.CreatedAt).ThenByDescending(e => e.Id).ToList();

            return response;
        }

        public async Task<List<ReturnInvoice>> GetNotSendedInvoices()
        {
            return await _db.ReturnInvoices.Where(e => e.Status == ReturnInvoiceStatus.Criada).ToListAsync();
        }

        public async Task UpdateAsync(ReturnInvoice returneInvoice)
        {
            _db.ReturnInvoices.Update(returneInvoice);
            await _db.SaveChangesAsync();            
        }
    }
}
