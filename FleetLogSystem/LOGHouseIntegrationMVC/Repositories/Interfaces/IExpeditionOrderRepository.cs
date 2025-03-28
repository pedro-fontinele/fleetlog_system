using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IExpeditionOrderRepository
    {
        Task<ExpeditionOrder> Add(ExpeditionOrder expeditionOrder);
        ExpeditionOrder AddOrder(ExpeditionOrder expeditionOrder);
        ExpeditionOrder GetById(int id);
        ExpeditionOrder GetOrderByInvoiceAccessKey(string invoiceAccessKey);
        Task<ExpeditionOrder> GetOrderByInvoiceAccessKeyAsync(string invoiceAccessKey);
        Task<ExpeditionOrder> GetOrderByInvoiceNumberAndClientId(int id, int invoiceNumber, int clientId);
        Task<ExpeditionOrder> GetOrderByIdAsync(int id);
        ExpeditionOrder Update(ExpeditionOrder order);
        Task<ExpeditionOrder> UpdateAsync(ExpeditionOrder order);
        Task<List<ExpeditionOrder>> GetAllById(IEnumerable<int> enumerable);
        Task<PaginationBase<ExpeditionOrder>> GetOrdersByClientIdAndbyStatusAsync(int id, List<ExpeditionOrderStatus> status, ExpeditionOrderFilterViewModel filter);
        Task<List<ExpeditionOrder>> GetAllToday(DateTime now);
        Task<PaginationBase<ExpeditionOrder>> GetAllOrdersByStatusAsync(List<ExpeditionOrderStatus> status, ExpeditionOrderFilterViewModel request);

        Task<PaginationBase<ExpeditionOrder>> GetAllManualOrdersAsync(ExpeditionOrderFilterViewModel request);

        IQueryable<ExpeditionOrder> GetQueryableFilterToManualOrders(ExpeditionOrderFilterViewModel filter);

        Task<ExpeditionOrder> GetByExternalNumber(ExpeditionOrder order, OrderOrigin origin);
        ExpeditionOrder GetByInvoiceAccessKey(string invoiceAccessKey);

        Task<PaginationBase<ExpeditionOrder>> GetByFilter(ExpeditionOrderFilterViewModel filter, List<ExpeditionOrderStatus> statusToFilter);

        Task<List<ExpeditionOrder>> GetAllOrdersByStatusIsNotAsync(List<ExpeditionOrderStatus> status);

        Task<List<ExpeditionOrder>> GetOrdersByClientIdAndbyStatusIsNotAsync(int id, List<ExpeditionOrderStatus> status);


        Task<int> CoutItens(PaginationBase<ExpeditionOrder> pag);
        List<int> GetOrdersIdsByPickingList(int id);

        Task<List<int>> GetOrdersIdsByPickingListAsync(int id);

        Task<List<ExpeditionOrder>> GetOrdersIsNotGeneratedReturnInvoice(DateTime maxDate);

        Task<List<ExpeditionOrder>> GetOrdersIsNotGeneratedReturnInvoiceByClientId(int clientId);

        List<ExpeditionOrder> GetByPackingListTransportationId(int packingListTransportationId);

        IQueryable<ExpeditionOrder> GetQueryableFilter(ExpeditionOrderFilterViewModel filter, List<ExpeditionOrderStatus> statusToFilter = null);

        Task<List<ExpeditionOrder>> GetOrdersByClientIdAndbyStatusAsync(int id, List<ExpeditionOrderStatus> status);

        Task<List<ExpeditionOrder>> GetAllOrdersByStatusAndDateIsNotAsync(List<ExpeditionOrderStatus> status, DateTime date);
    }
}
