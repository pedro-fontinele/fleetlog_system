using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class ReturnInvoiceProductInvoicesRepository : RepositoryBase, IReturnInvoiceProductInvoicesRepository
    {
        public async Task<ReturnInvoiceProductInvoices> Add(ReturnInvoiceProductInvoices invoice)
        {
            _db.ReturnInvoiceProductInvoices.Add(invoice);
            await _db.SaveChangesAsync();
            return invoice;
        }

        public async Task Delete(ReturnInvoiceProductInvoices data)
        {
            _db.ReturnInvoiceProductInvoices.Remove(data);
            await _db.SaveChangesAsync();
        }

        public async Task<List<ReturnInvoiceProductInvoices>> GetByAccessKey(string accessKey, int returnInvoiceId)
        {
            var items = await _db.ReturnInvoiceProductInvoices.Where(e => e.InvoiceAccessKey == accessKey && e.ReturnInvoiceId == returnInvoiceId).ToListAsync();            
            return items;
        }

        public async Task<ReturnInvoiceProductInvoices> GetByIdAsync(int id)
        {
            var items = await _db.ReturnInvoiceProductInvoices.Where(e => e.Id == id).FirstOrDefaultAsync();
            return items;
        }

        public async Task<List<ReturnInvoiceProductInvoices>> GetByReturnInvoiceId(int id)
        {
            var items = await _db.ReturnInvoiceProductInvoices.Where(e => e.ReturnInvoiceId == id).ToListAsync();
            return items;
        }
    }
}
