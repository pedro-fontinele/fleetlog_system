using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class AppVersionRepository : RepositoryBase, IAppVersionRepository
    {

        public async Task<AppVersion> GetByIdAsync(int id)
        {
            return await _db.AppVersions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<AppVersion> AddAsync(AppVersion appVersion)
        {
            await _db.AppVersions.AddAsync(appVersion);
            await _db.SaveChangesAsync(); 

            return appVersion;
        }

        public async Task<AppVersion> UpdateAsync(AppVersion appVersion)
        {
            _db.AppVersions.Update(appVersion);
            await _db.SaveChangesAsync();

            return appVersion;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            AppVersion app = await GetByIdAsync(id);
            _db.AppVersions.Remove(app);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
