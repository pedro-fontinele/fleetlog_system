using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using Org.BouncyCastle.Utilities;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IReceiptNoteItemService
    {
        ReceiptNoteItem SearchItemToValidate(int ReceiptNoteId, string code);
        byte[] GeneratePdfWithLabels(List<ReceiptNoteItemTransferViewModel> itens);

        Task<List<ReceiptNoteWithPositionViewModel>> GetReceiptNoteItemWithPosition(List<Product> items);

        byte[] GeneratePdfWithCaixMasterLabels(List<CaixaMasterLabelViewModel> itens);

        byte[] GeneratePdfWithIdentityLabels(List<string> itens);
    }
}
