using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IDevolutionAndReceiptNoteRepository
    {
        Task<List<DevolutionAndReceiptNote>> AddARangesync(List<DevolutionAndReceiptNote> items);
        Task<List<DevolutionAndReceiptNote>> GetByNoteId(int id);
        bool DeleteByReceiptNoteId(int id);
    }
}
