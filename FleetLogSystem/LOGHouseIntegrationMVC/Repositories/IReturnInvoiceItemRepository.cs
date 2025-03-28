using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories
{
    public interface IReturnInvoiceItemRepository
    {
        Task<List<ReturnInvoiceItem>> AddListAsync(List<ReturnInvoiceItem> items);
    }
}
