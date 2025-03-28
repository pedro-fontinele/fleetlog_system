using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IExpeditionOrdersLotNotFoundedService
    {
        Task<PaginationBase<ExpeditionOrdersLotNotFounded>> GetAllNotFoundedOrders(NotFoundedPaginationRequestViewModel request);

        Task<List<ExpeditionOrdersLotNotFounded>> GetExpeditionOrdersLotNotFoundedByExpeditionOrders(List<int> expeditionOrderIds);
        
        Task ChangeNotFoundStatus(int id, ExpeditionOrdersLotNotFoundedStatusEnum status);
    }
}
