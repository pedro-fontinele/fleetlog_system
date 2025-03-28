using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class ShippingCompanyRepository : RepositoryBase, IShippingCompanyRepository
    {

        public async Task<ShippingCompany> GetById(int id)
        {
            return await _db.ShippingCompanies.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<ShippingCompany>> GetAll()
        {
            return await _db.ShippingCompanies
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<ShippingCompany> Add(ShippingCompany shippingCompany)
        {
            _db.ShippingCompanies.Add(shippingCompany);
            _db.SaveChanges();

            return shippingCompany;
        }

        public async Task<ShippingCompany> Update(ShippingCompany shippingCompany)
        {
            var companyById = await GetById(shippingCompany.Id);

            if (companyById == null)
                throw new System.Exception("Houve um erro na atualização da transportadora");

            companyById.Name = shippingCompany.Name;
            companyById.Active = shippingCompany.Active;

            _db.ShippingCompanies.Update(companyById);
            await _db.SaveChangesAsync();

            return shippingCompany;
        }


    }
}
