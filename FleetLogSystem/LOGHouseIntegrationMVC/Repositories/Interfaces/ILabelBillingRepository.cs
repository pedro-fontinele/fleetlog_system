using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface ILabelBillingRepository
    {
        Task<LabelBilling> CreateAsync(LabelBilling labelBilling);
        Task<List<LabelBilling>> GetByReceiptNoteAsync(int receiptNoteId);
        bool DeleteByNoteId(int receiptNoteId);

    }
}
