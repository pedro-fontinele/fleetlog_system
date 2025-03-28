using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories
{
    public class ReturnInvoiceItemRepository : RepositoryBase, IReturnInvoiceItemRepository
    {
        public async Task<List<ReturnInvoiceItem>> AddListAsync(List<ReturnInvoiceItem> items)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {

                       await _db.ReturnInvoiceItems.AddRangeAsync(items);
                       await _db.SaveChangesAsync();

                    transaction.Commit();

                    return items;

                } catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Erro:" + ex);
                }
            }
        }
    }
}
