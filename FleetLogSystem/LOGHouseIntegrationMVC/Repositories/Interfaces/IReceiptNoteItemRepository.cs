using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using Org.BouncyCastle.Utilities;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IReceiptNoteItemRepository
    {
        ReceiptNoteItem GetById(int id);

        Task<ReceiptNoteItem> GetByIdAsync(int id);

        IQueryable<ReceiptNoteItem> GetAll();

        ReceiptNoteItem GetByCode(string code);

        ReceiptNoteItem? GetByReceiptNoteAndEAN(int receiptNoteId, string ean);

        List<ReceiptNoteItem> GetByReceiptNote(int id);

        ReceiptNoteItem Add(ReceiptNoteItem noteItem);

        List<ReceiptNoteItem> AddRange(List<ReceiptNoteItem> noteItems);

        ReceiptNoteItem Update(ReceiptNoteItem noteItem);
        bool Delete(int id);

        Task<List<ReceiptNoteItem>> GetReceiptNoteAndQuantitys(List<int> ids);
        Task<List<ReceiptNoteItem>> GetAllByStatusAndMinimusDate(List<NoteStatus> receiptNoteStatusNotUsable, DateTime baseDate, string code, int productId, int clientId);
        Task<ReceiptNoteItem> GetByReceiptNoteAndEanAsync(int receiptNoteId, string ean);
        Task<ReceiptNoteItem> GetByReceiptNoteAndCodeAsync(int receiptNoteId, string code);
        Task<List<ReceiptNoteItem>> GetAllWithoutReceiptLots();

    }
}
