using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services
{
    public class ShippingCompanyService : IShippingCompanyService
    {
        private readonly IShippingCompanyRepository _shippingCompanyRepository;

        public ShippingCompanyService(IShippingCompanyRepository shippingCompanyRepository)
        {
            _shippingCompanyRepository = shippingCompanyRepository;
        }

        public async Task<ShippingCompany> AddCompany(ShippingCompany shippingCompany)
        {
            if(shippingCompany == null) 
                throw new ArgumentNullException("Não foi possível atualizar a transportadora");

            return await _shippingCompanyRepository.Add(shippingCompany);
        }

        public async Task<List<ShippingCompany>> GetAll()
        {
            return await _shippingCompanyRepository.GetAll();
        }

        public async Task<ShippingCompany> GetById(int id)
        {
            if(id <= 0 )
                throw new ArgumentNullException("Não foi possível encontrar essa transportadora, por favor, tente novamente!");

            return await _shippingCompanyRepository.GetById(id);
        }

        public async Task<ShippingCompany> UpdateCompany(ShippingCompany shippingCompany)
        {
            if (shippingCompany == null)
                throw new ArgumentNullException("Não foi possível atualizar a transportadora");

            return await _shippingCompanyRepository.Update(shippingCompany);
        }
    }
}
