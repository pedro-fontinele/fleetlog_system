using LOGHouseSystem.Controllers.API.BarcodeColectorApi;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        Invoice GetById(int id);

        List<Invoice> GetAll();

        Invoice GetByAcessKey(string acessKey);

        Task<Invoice> AddAsync(Invoice note);

        List<Invoice> AddRange(List<Invoice> notes);

        Invoice Update(Invoice note);

        bool Delete(int id);

        Task<Invoice> GetByAcessKeyAsync(string? invoiceAccessKey);
        Task<Invoice> UpdateAsync(Invoice invoice);
    }
}
