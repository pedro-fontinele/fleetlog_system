using DocumentFormat.OpenXml.Office2010.Excel;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class ReceiptNoteItemRepository : RepositoryBase, IReceiptNoteItemRepository
    {
        public ReceiptNoteItem GetById(int id)
        {
            return _db.ReceiptNoteItems
                //.Include(x => x.ReceiptNote)
                .FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<ReceiptNoteItem> GetAll()
        {
            var query = _db.ReceiptNoteItems
                .AsNoTracking()
                .Select(receiptNoteItem => receiptNoteItem);
            return query;
        }

        public ReceiptNoteItem GetByCode(string code)
        {
            return _db.ReceiptNoteItems
                .Where(x => x.Code == code)
                .AsNoTracking()
                //.Include(x => x.ReceiptNote)
                .FirstOrDefault();
        }

        public ReceiptNoteItem? GetByReceiptNoteAndEAN(int receiptNoteId, string ean)
        {
            return _db.ReceiptNoteItems
                .Where(x => x.ReceiptNoteId == receiptNoteId && x.Ean == ean)
                .FirstOrDefault();
        }

        public List<ReceiptNoteItem> GetByReceiptNote(int id)
        {
            List<ReceiptNoteItem> noteItens = _db.ReceiptNoteItems
                                                 .Where(X => X.ReceiptNoteId == id)
                                                 .AsNoTracking()
                                                 .ToList();
            return noteItens;
        }


        public ReceiptNoteItem Add(ReceiptNoteItem noteItem)
        {
            _db.ReceiptNoteItems.Add(noteItem);
            _db.SaveChanges();
            return noteItem;
        }

        public List<ReceiptNoteItem> AddRange(List<ReceiptNoteItem> noteItems)
        {

            _db.ReceiptNoteItems.AddRange(noteItems);
            _db.SaveChanges();
            return noteItems;
        }

        public ReceiptNoteItem Update(ReceiptNoteItem noteItem)
        {
            var entry = _db.Entry(noteItem);

            if (entry.State == EntityState.Detached)
            {
                var set = _db.Set<ReceiptNoteItem>();
                ReceiptNoteItem attachedEntity = set.Find(noteItem.Id);  // You need to have access to key

                if (attachedEntity != null)
                {
                    var attachedEntry = _db.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(noteItem);
                }
                else
                {
                    entry.State = EntityState.Modified; // This should attach entity
                }
            }

            _db.SaveChanges();
            return noteItem;
        }

        public bool Delete(int id)
        {
            ReceiptNoteItem noteItemDb = GetById(id);
            if (noteItemDb == null) throw new Exception("Houve um erro na deleção da nota");

            _db.ReceiptNoteItems.Remove(noteItemDb);
            _db.SaveChanges();
            return true;
        }

        public async Task<List<ReceiptNoteItem>> GetReceiptNoteAndQuantitys(List<int> ids)
        {
            return await _db.ReceiptNoteItems.Where(x => ids.Any(y => y == x.Id)).ToListAsync();
        }

        public async Task<ReceiptNoteItem> GetByIdAsync(int id)
        {
            return await _db.ReceiptNoteItems.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ReceiptNoteItem> GetByReceiptNoteAndEanAsync(int receiptNoteId, string ean)
        {
            return await _db.ReceiptNoteItems.FirstOrDefaultAsync(x => x.ReceiptNoteId == receiptNoteId && x.Ean == ean);
        }

        public async Task<List<ReceiptNoteItem>> GetAllWithoutReceiptLots()
        {
            return await _db.ReceiptNoteItems.Include(x => x.ReceiptNote).ThenInclude(x => x.Client).Where(x => x.LotGenerated == YesOrNo.No 
            && (x.ReceiptNote.Status == NoteStatus.AguardandoEnderecamento || x.ReceiptNote.Status == NoteStatus.Finalizada))
                .OrderBy(e => e.ReceiptNote.EntryDate).ToListAsync();
        }

        public async Task<ReceiptNoteItem> GetByReceiptNoteAndCodeAsync(int receiptNoteId, string code)
        {
            return await _db.ReceiptNoteItems.FirstOrDefaultAsync(x => x.ReceiptNoteId == receiptNoteId && x.Code == code);
        }

        public async Task<List<ReceiptNoteItem>> GetAllByStatusAndMinimusDate(List<NoteStatus> status, DateTime baseDate, string code, int productId, int clientId)
        {
            List<int> enumIntValues = status.Select(a => (int)a).ToList();

            var data = await _db.ReceiptNoteItems.Include(e => e.ReceiptNote)
            .Where(x => enumIntValues.Contains((int)x.ReceiptNote.Status) && x.ReceiptNote.IssueDate >= baseDate && x.ReceiptNote.ClientId == clientId 
                && ((x.Code == code && (x.ProductId == 0 || x.ProductId == null)) || x.ProductId == productId) )
            .ToListAsync();

            return data;
        }
    }
}
