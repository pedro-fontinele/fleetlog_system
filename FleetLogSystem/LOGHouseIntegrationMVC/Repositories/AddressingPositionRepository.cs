using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IO;

namespace LOGHouseSystem.Repositories
{
    public class AddressingPositionRepository : RepositoryBase, IAddressingPositionRepository
    {


        public List<AddressingPosition> GetAll()
        {
           return _db.AddressingPositions
                .AsNoTracking()
                .ToList();
        }

        public AddressingPosition GetById(int id)
        {
            return _db.AddressingPositions
                      .FirstOrDefault(x => x.AddressingPositionID == id);
        }

        public List<AddressingPosition> GetByStreetId(int id)
        {
            return _db.AddressingPositions
                      .Where(x => x.AddressingStreetID == id)
                      .ToList();

        }

        public AddressingPosition Add(AddressingPosition ads)
        {
            _db.AddressingPositions.Add(ads);
            _db.SaveChanges();

            return ads;
        }

        public AddressingPosition Update(AddressingPosition ads)
        {
            AddressingPosition adsById = GetById(ads.AddressingPositionID);

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

            AddressingPosition ads = GetById(id);

            _db.AddressingPositions.Remove(ads);
            _db.SaveChanges();

            return true;
        }

        
        public bool DeleteArrange(List<int> ids)
        {
            if (ids == null)
                return false;

            foreach (int id in ids)
            {
                AddressingPosition ads = GetById(id);

                _db.AddressingPositions.Remove(ads);
                _db.SaveChanges();

            }

            return true;

        }

        public List<AddressingPosition> SearchByName(string addressingPositionName)
        {
            return _db.AddressingPositions.Where(position => position.Name == addressingPositionName).ToList();
        }
    }
}
