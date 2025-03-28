using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;

namespace LOGHouseSystem.Repositories
{
    public class ShippingDetailsRepository : RepositoryBase, IShippingDetailsRepository
    {

        public ShippingDetailsRepository(AppDbContext appDb) : base(appDb)
        {

        }

        public ShippingDetails Add(ShippingDetails shippingDetails)
        {
            _db.Add(shippingDetails);
            _db.SaveChanges();
            return shippingDetails;
        }

        public ShippingDetails Update(ShippingDetails details)
        {
            _db.Update(details);
            _db.SaveChanges();
            return details;
        }
    }
}
