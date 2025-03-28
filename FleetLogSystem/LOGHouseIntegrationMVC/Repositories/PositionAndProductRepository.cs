using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class PositionAndProductRepository : RepositoryBase, IPositionAndProductRepository
    {
        public PositionAndProduct Add(PositionAndProduct posAndProd)
        {
            _db.PositionsAndProducts.Add(posAndProd);
            _db.SaveChanges();

            return posAndProd;
        }

        public void AddRange(List<PositionAndProduct> positionAndProductsToAdd)
        {
            _db.PositionsAndProducts.AddRange(positionAndProductsToAdd);
            _db.SaveChanges();
        }

        public bool ProductAlreadyAssociated(int addressPositionId, int productId)
        {
            return _db.PositionsAndProducts.Any(positionAndProduct => positionAndProduct.ProductId == productId && positionAndProduct.AddressingPositionId == addressPositionId);
        }

        public async Task<PositionAndProduct> GetFirstProductAddressAsync(int productId)
        {
            return await _db.PositionsAndProducts.Include(e => e.AddressingPosition).Where(e => e.ProductId == productId).FirstOrDefaultAsync();
        }

        public async Task<PositionAndProduct> GetLastProductAddressAsync(int productId)
        {
            return await _db.PositionsAndProducts.Include(e => e.AddressingPosition).Where(e => e.ProductId == productId).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        public async Task Update(PositionAndProduct posAndProd)
        {
            if (posAndProd == null)
                throw new Exception("Não foi possível atualizar essa posição, por favor, tente novamente!");

            _db.Update(posAndProd);
            
            await _db.SaveChangesAsync();
        }
    }
}
