namespace LOGHouseSystem.Services.Interfaces
{
    public interface IDevolutionAndReceiptNoteService
    {

        Task FinalizeDevolutionsByNoteIdAnd(int noteId);
    }
}
