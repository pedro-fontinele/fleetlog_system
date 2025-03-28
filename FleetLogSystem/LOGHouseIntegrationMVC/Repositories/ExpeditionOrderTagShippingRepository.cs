using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class ExpeditionOrderTagShippingRepository : RepositoryBase, IExpeditionOrderTagShippingRepository
    {
        public ExpeditionOrderTagShippingRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public ExpeditionOrderTagShipping Add(ExpeditionOrderTagShipping orderShipping)
        {
            orderShipping.EntryDate = DateTime.Now;
            _db.ExpeditionOrderTagShipping.Add(orderShipping);
            _db.SaveChanges();

            return orderShipping;
        }

        public async Task<ExpeditionOrderTagShipping> AddAsync(ExpeditionOrderTagShipping orderShipping)
        {
            orderShipping.EntryDate = DateTime.Now;
            _db.ExpeditionOrderTagShipping.Add(orderShipping);
            await _db.SaveChangesAsync();

            return orderShipping;
        }

        public async Task<ExpeditionOrderTagShipping> GetShippingByExpeditionOrderIdAsync(int id)
        {
            return await _db.ExpeditionOrderTagShipping.Where(e => e.ExpeditionOrderId == id).FirstOrDefaultAsync();
        }

        public async Task<ExpeditionOrderTagShipping?> GetShippingByInvoiceAccessKeyAsync(string accessKey)
        {
            return await _db.ExpeditionOrderTagShipping.Where(e => e.InvoiceAccessKey == accessKey).FirstOrDefaultAsync();
        }

        public async Task<ExpeditionOrderTagShipping> GetShippingByShippingCode(string order)
        {
            return await _db.ExpeditionOrderTagShipping.Where(e => e.ShippingCode == order).FirstOrDefaultAsync();
        }

        public ExpeditionOrderTagShipping Update(ExpeditionOrderTagShipping orderShipping)
        {
            _db.ExpeditionOrderTagShipping.Update(orderShipping);
            _db.SaveChanges();

            return orderShipping;
        }

        public async Task<ExpeditionOrderTagShipping> UpdateAsync(ExpeditionOrderTagShipping orderShipping)
        {
            _db.ExpeditionOrderTagShipping.Update(orderShipping);            
            await _db.SaveChangesAsync();

            return orderShipping;
        }

        public ExpeditionOrderTagShipping GetShippingByExpeditionOrderId(int id)
        {
            return _db.ExpeditionOrderTagShipping.Where(e => e.ExpeditionOrderId == id).FirstOrDefault();
        }

    }
}
