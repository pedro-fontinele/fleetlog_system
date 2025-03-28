using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<Invoice> AddAsync(Invoice invoice);
        Task<Invoice> GetByAcessKeyAsync(string acessKey);
        bool DeleteById(int id);
    }
}
