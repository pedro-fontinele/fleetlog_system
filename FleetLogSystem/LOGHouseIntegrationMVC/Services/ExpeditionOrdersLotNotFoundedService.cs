using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Services
{
    public class ExpeditionOrdersLotNotFoundedService : IExpeditionOrdersLotNotFoundedService
    {
        private IExpeditionOrdersLotNotFoundedRepository _expeditionOrdersLotNotFoundedRepository;

        public ExpeditionOrdersLotNotFoundedService(IExpeditionOrdersLotNotFoundedRepository expeditionOrdersLotNotFoundedRepository)
        {
            _expeditionOrdersLotNotFoundedRepository = expeditionOrdersLotNotFoundedRepository;
        }

        public async Task ChangeNotFoundStatus(int id, ExpeditionOrdersLotNotFoundedStatusEnum status)
        {
            ExpeditionOrdersLotNotFounded item = await _expeditionOrdersLotNotFoundedRepository.GetByIdAsync(id);

            item.Status = status;

            await _expeditionOrdersLotNotFoundedRepository.UpdateAsync(item);
        }

        public async Task<PaginationBase<ExpeditionOrdersLotNotFounded>> GetAllNotFoundedOrders(NotFoundedPaginationRequestViewModel request)
        {
            var data = await _expeditionOrdersLotNotFoundedRepository.GetAllNotFoundedOrders(request);

            return data;
        }

        public async Task<List<ExpeditionOrdersLotNotFounded>> GetExpeditionOrdersLotNotFoundedByExpeditionOrders(List<int> expeditionOrderIds)
        {
            List<ExpeditionOrdersLotNotFounded> lotsNotFounded = await _expeditionOrdersLotNotFoundedRepository.GetByExpeditionOrderIdsAndStatus(expeditionOrderIds, ExpeditionOrdersLotNotFoundedStatusEnum.Created);

            return lotsNotFounded;
        }
    }
}
