using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class DevolutionImageRepository : RepositoryBase, IDevolutionImageRepository
    {
        public async Task AddImageAsync(DevolutionImage image)
        {
            await _db.DevolutionImages.AddAsync(image);
            await _db.SaveChangesAsync();
        }

        public async Task<List<DevolutionImage>> GetAllImagesByDevolutionIdAsync(int id)
        {
            return await _db.DevolutionImages.Include(x => x.Devolution).Where(x => x.DevolutionId == id).ToListAsync();
        }
    }
}
