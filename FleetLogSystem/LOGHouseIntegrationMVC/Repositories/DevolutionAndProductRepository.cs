using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class DevolutionAndProductRepository : RepositoryBase, IDevolutionAndProductRepository
    {
        public async Task<List<DevolutionAndProduct>> AddAllAsync(int devolutionId, List<int> productIds)
        {
            List< DevolutionAndProduct > list = new List< DevolutionAndProduct >();

            foreach (var id in productIds)
            {
                DevolutionAndProduct product = new DevolutionAndProduct()
                {
                    DevolutionId = devolutionId,
                    ProductId = id,
                };

                list.Add(product);
            }

           await _db.DevolutionAndProducts.AddRangeAsync(list);
           await _db.SaveChangesAsync();

            return list;
        }

        public async Task<List<DevolutionAndProduct>> GetAllByDevolutionIdAsync(int devolutionId)
        {
          return await _db.DevolutionAndProducts.Include(x => x.Product).Where(X => X.DevolutionId == devolutionId).ToListAsync();
        }
    }
}
