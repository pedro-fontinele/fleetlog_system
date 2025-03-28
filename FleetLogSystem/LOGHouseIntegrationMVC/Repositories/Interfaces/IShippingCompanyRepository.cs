using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IShippingCompanyRepository
    {
        Task<List<ShippingCompany>> GetAll();
        Task<ShippingCompany> GetById(int id);
        Task<ShippingCompany> Add(ShippingCompany shippingCompany);
        Task<ShippingCompany> Update(ShippingCompany shippingCompany);
    }
}
