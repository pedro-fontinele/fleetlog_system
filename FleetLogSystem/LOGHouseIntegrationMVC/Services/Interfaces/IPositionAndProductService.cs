using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IPositionAndProductService
    {
        Models.PositionAndProduct AssociateProductToPosition(int productId, string addressPosition);
    }
}
