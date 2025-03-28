using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IReturnInvoiceRepository
    {
        Task<PaginationBase<ReturnInvoice>> GetByPagination(PaginationRequest request);
        Task<List<ReturnInvoice>> GetAllById(List<int> returnInvoices);
        Task<ReturnInvoice> GetByIdAsync(int returnInvoice);
        Task UpdateAsync(ReturnInvoice returneInvoice);
        Task<ReturnInvoice> GetByExternalId(string externalId);
        Task<ReturnInvoice> Add(ReturnInvoice invoice);
        Task<List<ReturnInvoice>> GetNotSendedInvoices();
    }
}
