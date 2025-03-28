using LOGHouseSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IPositionAndProductRepository
    {
        PositionAndProduct Add(PositionAndProduct posAndProd);
        void AddRange(List<PositionAndProduct> positionAndProductsToAdd);
        Task Update(PositionAndProduct posAndProd);
        bool ProductAlreadyAssociated(int positionId, int productId);
        Task<PositionAndProduct> GetFirstProductAddressAsync(int productId);
        Task<PositionAndProduct> GetLastProductAddressAsync(int productId);
    }
}
