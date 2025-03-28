using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IShippingDetailsRepository
    {
        ShippingDetails Add(ShippingDetails shippingDetails);
        ShippingDetails Update(ShippingDetails details);
    }
}
