using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IPickingListItemRepository
    {
        Task<PickingListItem> Create(PickingListItem picking);
        IQueryable<PickingListItem> GetAll();
        PickingListItem Update(PickingListItem pickingListItem);

        Task<bool> CancelByPickingListIdAsync(int id);

        List<PickingListItem> GetByPickingListId(int id);
        Task<PickingListItem> GetByProductIdAsync(int? productId, int id);
    }
}
