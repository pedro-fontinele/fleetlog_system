using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IReturnInvoiceProductInvoicesRepository
    {
        Task<ReturnInvoiceProductInvoices> Add(ReturnInvoiceProductInvoices invoice);
        Task Delete(ReturnInvoiceProductInvoices data);
        Task<List<ReturnInvoiceProductInvoices>> GetByAccessKey(string accessKey, int returnInvoiceId);
        Task<ReturnInvoiceProductInvoices> GetByIdAsync(int id);
        Task<List<ReturnInvoiceProductInvoices>> GetByReturnInvoiceId(int id);
    }
}
