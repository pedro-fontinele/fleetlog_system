using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IReturnInvoiceOrdersRepository
    {
        Task<ReturnInvoiceOrders> AddAsync(ReturnInvoiceOrders order);
        Task<List<ReturnInvoiceOrders>> GetByExpeditionOrderId(int expeditionOrderId);
        Task<List<ReturnInvoiceOrders>> GetByReturnInvoiceId(int returnInvoiceId);
    }
}
