using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using PagedList;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IPickingListRepository
    {
        List<PickingList> GetByStatus(PickingListStatus status);
        Task<PickingList> Create(PickingList picking);
        PickingList GetById(int id);
        PickingList Update(PickingList pickingList);

        List<PickingList> GetAllWithStatusGeradoAndEmAtendimento();
        PagedList<PickingListWithUrlAndArrayByteViewModel> GetAllPaged(int page, int pageSize, int? cartId, int? invoiceNumber);

        Task<PickingList> GetByOrderExpeditionIdAsync(int id);

        bool CancelById(int id);
        Task<PickingList> GetByIdAsync(int? orderId);
        PickingList GetLastByCartId(int cartId);
    }
}
