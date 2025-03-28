using LOGHouseSystem.Controllers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Services
{
    public class InvoiceService : IInvoiceService
    {

            public IInvoiceRepository _invoiceRepository;

            public InvoiceService(IInvoiceRepository invoiceRepository)
            {
                _invoiceRepository = invoiceRepository;
            }

            public async Task<Invoice> AddAsync(Invoice invoice) 
            {
                return await _invoiceRepository.AddAsync(invoice);
            }

            public bool DeleteById(int id)
            {
                var result = _invoiceRepository.Delete(id);

                if (result == false)
                    throw new Exception("Houve um erro na deleção da nota");

                return result;
            }

            public async Task<Invoice> GetByAcessKeyAsync(string acessKey)
            {
                return _invoiceRepository.GetByAcessKey(acessKey);
            }
    }
}
