using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface ITinyOrdersRepository
    {
        Task<TinyOrder> Add(TinyOrder tinyOrder);
    }
}
