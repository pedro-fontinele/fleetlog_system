using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services
{
    public class DevolutionAndReceiptNoteService : IDevolutionAndReceiptNoteService
    {
        private readonly IDevolutionRepository _devolutionRepository;
        private readonly IDevolutionAndReceiptNoteRepository _devolutionAndReceiptNoteRepository;
        protected readonly AppDbContext _db;

        public DevolutionAndReceiptNoteService(IDevolutionAndReceiptNoteRepository devolutionAndReceiptNoteRepository, IDevolutionRepository devolutionRepository, AppDbContext db = null)
        {
            _devolutionAndReceiptNoteRepository = devolutionAndReceiptNoteRepository;
            _devolutionRepository = devolutionRepository;
            _db = db ?? new AppDbContext();

        }

        public async Task FinalizeDevolutionsByNoteIdAnd(int noteId)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    List<DevolutionAndReceiptNote> devAndNotes = await _devolutionAndReceiptNoteRepository.GetByNoteId(noteId);

                    if (devAndNotes.Count > 0)
                    {
                        foreach (var dev in devAndNotes)
                        {
                            Devolution devolution = _devolutionRepository.GetDevolutionById(dev.DevolutionId);
                            devolution.Status = Infra.Enums.DevolutionStatus.Finalizado;
                            await _devolutionRepository.UpdateAsync(devolution);
                        }
                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new Exception("Não foi possível finalizar esse recebimento - falha na atualização");
                }
            }
        }
    }
}
