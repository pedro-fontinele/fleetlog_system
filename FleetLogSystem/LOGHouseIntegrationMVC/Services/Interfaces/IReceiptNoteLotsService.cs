using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using static LOGHouseSystem.Services.ReceiptNoteLotsService;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IReceiptNoteLotsService
    {
        public Task<NoteListResponse> ProcessLotAsync(int idProduct, List<ExpeditionOrderItem> items, double quantity);
        public ReceiptNoteLots? GetReceiptNote(int idProduto);        
        public void CreateNewLot(int product, ReceiptNoteItem receiptNote, double inputQuantity);
        Task CreateLots(ReceiptNoteItem productReceipt);
        Task<PaginationBase<ReceiptNoteLots>> GetAllLots(ReceiptNoteIndexLotsPaginationRequest request);
        Task UpdateStatus(int id, LotStatus status);
        Task ActiveStatus(int id);
    }
}
