using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IShippingCompanyService
    {
        Task<ShippingCompany> GetById(int id);
        Task<ShippingCompany> AddCompany(ShippingCompany shippingCompany);
        Task<ShippingCompany> UpdateCompany(ShippingCompany shippingCompany);
        Task<List<ShippingCompany>> GetAll();
    }
}
