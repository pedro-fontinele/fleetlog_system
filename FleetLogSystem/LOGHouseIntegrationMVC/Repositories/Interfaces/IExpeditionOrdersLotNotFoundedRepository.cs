using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IExpeditionOrdersLotNotFoundedRepository
    {
        Task<ExpeditionOrdersLotNotFounded> AddAsync(ExpeditionOrdersLotNotFounded order);
        Task<List<ExpeditionOrdersLotNotFounded>> GetAllByProductIdAndStatus(int productId, ExpeditionOrdersLotNotFoundedStatusEnum created);
        Task<PaginationBase<ExpeditionOrdersLotNotFounded>> GetAllNotFoundedOrders(NotFoundedPaginationRequestViewModel request);
        Task<List<ExpeditionOrdersLotNotFounded>> GetByClientIdAndStatus(int clientId, ExpeditionOrdersLotNotFoundedStatusEnum status);
        Task<List<ExpeditionOrdersLotNotFounded>> GetByExpeditionOrderIds(List<int> expeditionOrderIds);
        Task<List<ExpeditionOrdersLotNotFounded>> GetByExpeditionOrderIdsAndStatus(List<int> expeditionOrderIds, ExpeditionOrdersLotNotFoundedStatusEnum status);
        Task<ExpeditionOrdersLotNotFounded> GetByIdAsync(int id);
        Task UpdateAsync(ExpeditionOrdersLotNotFounded item);
    }
}
