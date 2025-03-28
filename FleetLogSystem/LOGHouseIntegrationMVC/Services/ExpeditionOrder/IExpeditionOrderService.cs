using LOGHouseSystem.Controllers;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.Services.Helper;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Services
{
    public interface IExpeditionOrderService
    {
        Task<ExpeditionOrder> UpdateOrder(int id, ExpeditionOrderStatus newState, DateTime? finalizeDate = null);        
        Task ProcessTag(ExpeditionOrder order);
        Task ProcessTag(int id);

        Task<bool> CheckIfOrderExistsByInvoiceNumberAndClientId(int id, int invoiceNumber, int clientId);

        Task<ExpeditionOrder> AddOrderAndUpdateInvoice(ExpeditionOrder order);

        Task<PaginationBase<ExpeditionOrderWithPickingListViewModel>> GetByFilter(ExpeditionOrderFilterViewModel filter, List<ExpeditionOrderStatus> statusToFilter);
        Task<PickingList> GeneratePickingList(List<ExpeditionOrder> orders, string obs, PriorityEnum priority, MarketPlaceEnum marketplace);
        Task<bool> CheckIfOrderExistsByExternalNumber(ExpeditionOrder expeditionOrder, OrderOrigin origin);
        Task<PaginationBase<ExpeditionOrderWithPickingListViewModel>> GetAllByUserAndStatus(List<ExpeditionOrderStatus> status, ExpeditionOrderFilterViewModel request);
        Task<PaginationBase<ExpeditionOrderWithPickingListViewModel>> GetAllManualOrders(ExpeditionOrderFilterViewModel? request);

        Task<ExpeditionOrder> CreateOrderByNfe(ResponseDTO responseDTO);
        Task<byte[]> GetProcessedTag(ExpeditionOrderTagShipping tag);
        Task<ExpeditionOrder> SetVolumeQuantity(string invoiceAccessKey, int volumeQuantities);

        Task<List<ExpeditionOrderGroup>> GetGroupOfOrdersWithIds();        

        decimal? GetTotalValueInListOfExpeditionOrders(List<ExpeditionOrder> orders);

        (string, FileFormatEnum) SaveTagUploaded(IFormFile file, ExpeditionOrder order);
        Task Approve(int id);
        Task Cancel(int id, string subject,  string? message);
        Task<byte[]> GenerateSimplifiedDanfe(List<int> list);

        Task<List<FileConvert>> GenerateFilesConvert(List<ExpeditionOrder> list);

        Task<List<string>> GenerateMelhorEnvioUrl(List<ExpeditionOrder> list);

        Task<List<ExpeditionOrder>> GetOrdersByPickinIds(List<int> ids);

        Task<List<List<string>>> CheckIfOrderIsMelhorEnvio(List<int> ids);

        List<int> GetTagBlockedOrderIds();

        Task<List<ClientOrdersViewModel>> GetOrdersIsNoGeneratedReturnInvoiceGroupedByClient();
        Task<List<ExpeditionOrder>> GetPendentingReturnOrdersByClientId(int clientId);
        Task<List<ClientOrdersViewModel>> GetOrdersIsNoGeneratedReturnInvoiceGroupedByClientAndDate(DateTime maxDate);
    }

}

