using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class LabelBillingRepository : RepositoryBase, ILabelBillingRepository
    {
        public async Task<LabelBilling> CreateAsync(LabelBilling labelBilling)
        {
            if(labelBilling == null) 
                throw new Exception("Não foi possível adicionar a cobrança de etiquetas na base de dados");

            _db.LabelBillings.Add(labelBilling);
           await _db.SaveChangesAsync();

            return labelBilling;
        }

        public async Task<List<LabelBilling>> GetByReceiptNoteAsync(int receiptNoteId)
        {
            return await _db.LabelBillings.Where(e => e.ReceiptNoteId == receiptNoteId).ToListAsync();
        }

        public bool DeleteByNoteId(int receiptNoteId)
        {
            var labelBillingsToDelete = _db.LabelBillings.Where(x => x.ReceiptNoteId == receiptNoteId).ToList();

            if (labelBillingsToDelete.Count > 0)
            {
                _db.LabelBillings.RemoveRange(labelBillingsToDelete);
                _db.SaveChanges();
                return true;
            }

            return false;
        }

    }
}
