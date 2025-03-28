using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IReturnInvoiceService
    {
        Task<PaginationBase<ReturnInvoice>> GetAllByPaginationBase(PaginationRequest request);
        Task<ReturnInvoice> AddAsync(ReturnInvoice invoice);
        Task<List<ReturnInvoice>> AddReturnInvoiceAndItems(List<ExpeditionOrder> expeditionOrders);
        Task<ReturnInvoiceCompleteViewModel> GetReturnInvoiceCompleteData(int returnInvoiceId);
        Task<List<ReturnInvoiceOrdersResponseViewModel>> GetReturnInvoicesFromOrder(int expeditionOrderId);
        Task AddXmlFile(List<IFormFile> file, int returnInvoiceId);
        Task DeleteAttachedXml(int id);
    }
}
