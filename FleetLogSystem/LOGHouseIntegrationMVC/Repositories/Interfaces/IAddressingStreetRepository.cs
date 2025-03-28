using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IAddressingStreetRepository
    {
        List<AddressingStreet> GetAll();

        AddressingStreet GetById(int id);

        AddressingStreet Add(AddressingStreet ads);

        AddressingStreet Update(AddressingStreet ads);

        bool CheckByName(string name);

        bool Delete(int id);

    }
}
