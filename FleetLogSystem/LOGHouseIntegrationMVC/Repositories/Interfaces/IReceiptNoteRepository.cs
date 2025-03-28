using LOGHouseSystem.Controllers.API.BarcodeColectorApi;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using PagedList;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IReceiptNoteRepository
    {
        ReceiptNote GetById(int id);

        List<ReceiptNote> GetAll();

        ReceiptNote GetByAcessKey(string acessKey);

        List<ReceiptNote> GetByEntryDate(DateTime date);

        List<ReceiptNote> GetByStatus(NoteStatus noteStatus);
        List<ReceiptNote> GetDevolutionsNote();

        List<ReceiptNote> GetByClient();

        ReceiptNote Add(ReceiptNote note);

        List<ReceiptNote> AddRange(List<ReceiptNote> notes);

        ReceiptNote Update(ReceiptNote note);

        bool Delete(int id);

        List<ReceiptNote> GetByFilter(BarcodeColectorFilter filter);

        PagedList<ReceiptNote> GetByFilters(FilterViewModel filter);
        Task<ReceiptNote> GetByAcessKeyAsync(string? invoiceAccessKey);
        Task<List<ReceiptNote>> GetAllById(IEnumerable<int> receiptNotesIds);

        IQueryable<ReceiptNote> GetQueryableFilter(FilterViewModel filter);
        Task<ReceiptNote> GetByIdAsync(int receiptNoteId);
        Task<ReceiptNote> GetLastReceiptNoteAsync(int? clientId);
    }
}
