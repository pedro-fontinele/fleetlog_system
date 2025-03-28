using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using PagedList;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IPickingListService
    {
        bool CancelById(int id);
        List<PickingList> GetWithStatusGeradoAndEmAndamento();

        PagedList<PickingListWithUrlAndArrayByteViewModel> GetAllPaged(int Page, int? cartId, int? invoiceNumber);
        Task<byte[]> GenerateSimplifiedDanfePickingList(List<int> orders);

        Task<List<List<string>>> CheckIfExistsMelhorEnvioOrder(List<int> ids);

        PickingList GetLastByCartId(int cartId);

        Task Cancel(int id);
        Task<List<PickingListExpeditionOrdersViewModel>> GetOrdersByPickingListId(int id);
    }
}
