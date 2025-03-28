using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class ReturnInvoiceOrdersRepository : RepositoryBase, IReturnInvoiceOrdersRepository
    {
        public async Task<ReturnInvoiceOrders> AddAsync(ReturnInvoiceOrders order)
        {
            order.EntryDate = DateTime.Now;
            _db.ReturnInvoiceOrders.Add(order);
            await _db.SaveChangesAsync();
            return order;
        }

        public async Task<List<ReturnInvoiceOrders>> GetByExpeditionOrderId(int expeditionOrderId)
        {
            return await _db.ReturnInvoiceOrders.Where(e => e.ExpeditionOrderId == expeditionOrderId).Include(e => e.ReturnInvoice).Include(e => e.Product).ToListAsync();
        }

        public async Task<List<ReturnInvoiceOrders>> GetByReturnInvoiceId(int returnInvoiceId)
        {
            return await _db.ReturnInvoiceOrders.Include(e => e.Product).Include(e => e.ExpeditionOrder).Where(e => e.ReturnInvoiceId == returnInvoiceId).ToListAsync();
        }
    }
}
