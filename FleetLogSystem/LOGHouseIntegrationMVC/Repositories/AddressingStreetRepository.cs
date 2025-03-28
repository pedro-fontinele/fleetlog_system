using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class AddressingStreetRepository : RepositoryBase, IAddressingStreetRepository
    {

        public List<AddressingStreet> GetAll()
        {
           return _db.AddressingStreets
                .AsNoTracking()
                .Include(x => x.Positions)
                .ToList();

        }

        public AddressingStreet GetById(int id)
        {
            return _db.AddressingStreets
                      .FirstOrDefault(x => x.AddressingStreetID == id);
        }


        public AddressingStreet Add(AddressingStreet ads)
        {
            _db.AddressingStreets.Add(ads);
            _db.SaveChanges();

            return ads;
        }

        public AddressingStreet Update(AddressingStreet ads)
        {
            AddressingStreet adsById = GetById(ads.AddressingStreetID);

            ads.Status = adsById.Status;
            ads.Name = adsById.Name;

            _db.Update(ads);
            _db.SaveChanges();

            return ads;

        }

        public bool Delete(int id)
        {
            if (id == 0)
                return false;

            AddressingStreet ads = GetById(id);

            _db.AddressingStreets.Remove(ads);
            _db.SaveChanges();

            return true;
        }

        public bool CheckByName(string name)
        {
            var street = _db.AddressingStreets.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());

            if(street == null)
                return true;

            return false;
        }
    }
}
