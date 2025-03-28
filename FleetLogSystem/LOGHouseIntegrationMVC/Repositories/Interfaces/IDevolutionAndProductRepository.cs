using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IDevolutionAndProductRepository
    {
        Task<List<DevolutionAndProduct>> AddAllAsync(int devolutionId, List<int> productIds);

        Task<List<DevolutionAndProduct>> GetAllByDevolutionIdAsync(int devolutionId);

    }
}
