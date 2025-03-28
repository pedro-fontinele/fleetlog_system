using LOGHouseSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IAddressingPositionRepository
    {
        List<AddressingPosition> GetAll();


        AddressingPosition GetById(int id);


        List<AddressingPosition> GetByStreetId(int id);


        AddressingPosition Add(AddressingPosition ads);


        AddressingPosition Update(AddressingPosition ads);


        bool Delete(int id);

        
        bool DeleteArrange(List<int> ids);
        List<AddressingPosition> SearchByName(string addressingPositionName);
    }
}
