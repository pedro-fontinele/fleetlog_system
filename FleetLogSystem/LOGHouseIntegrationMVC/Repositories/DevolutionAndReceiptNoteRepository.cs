using DocumentFormat.OpenXml.ExtendedProperties;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class DevolutionAndReceiptNoteRepository : RepositoryBase, IDevolutionAndReceiptNoteRepository
    { 
        private readonly IDevolutionRepository _devolutionRepository;

        public DevolutionAndReceiptNoteRepository(IDevolutionRepository devolutionRepository)
        {
            _devolutionRepository = devolutionRepository;
        }



        public async Task<List<DevolutionAndReceiptNote>> AddARangesync(List<DevolutionAndReceiptNote> items)
        {

           await _db.DevolutionAndReceiptNote.AddRangeAsync(items);
           await _db.SaveChangesAsync();

            return items;

        }

        public bool DeleteByReceiptNoteId(int id)
        {
            List<DevolutionAndReceiptNote> devAndNotes = _db.DevolutionAndReceiptNote.Where(x => x.ReceiptNoteId == id).ToList();

            if(devAndNotes.Count > 0)
            {
                _db.DevolutionAndReceiptNote.RemoveRange(devAndNotes);
                _db.SaveChanges();
                return true;
            }

            return false;


        }

        public async Task<List<DevolutionAndReceiptNote>> GetByNoteId(int id)
        {
            return await _db.DevolutionAndReceiptNote.Where(x => x.ReceiptNoteId == id).ToListAsync();
        }



       
    }
}
