using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IBlingOrdersRepository
    {
        Task<BlingOrder> Add(BlingOrder order);
    }
}
