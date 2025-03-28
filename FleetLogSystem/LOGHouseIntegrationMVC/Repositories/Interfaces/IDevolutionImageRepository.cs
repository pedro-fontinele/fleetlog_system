using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IDevolutionImageRepository
    {
        Task AddImageAsync(DevolutionImage image);

        Task<List<DevolutionImage>> GetAllImagesByDevolutionIdAsync(int id);
    }
}
