using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IAppVersionRepository
    {
        Task<AppVersion> AddAsync(AppVersion appVersion);
        Task<AppVersion> UpdateAsync(AppVersion appVersion);
        Task<bool> DeleteByIdAsync(int id);
        Task<AppVersion> GetByIdAsync(int id);
    }
}
