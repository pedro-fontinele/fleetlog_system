using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.HangFire;
using LOGHouseSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Web.Mvc;

namespace LOGHouseSystem.Services
{
    public class ReceptNoteLotsRoutine : IReceptNoteLotsRoutine
    {
        private IReceiptNoteRepository _receiptNoteRepository;
        private IReceiptNoteLotsService _receiptNoteLotsService;
        private IReceiptNoteItemRepository _receiptNoteItemRepository;

        public ReceptNoteLotsRoutine(
            IReceiptNoteRepository receiptNoteRepository,
            IReceiptNoteLotsService receiptNoteLotsService,
            IReceiptNoteItemRepository receiptNoteItemRepository
            )
        {
            _receiptNoteRepository = receiptNoteRepository;
            _receiptNoteLotsService = receiptNoteLotsService;
            _receiptNoteItemRepository = receiptNoteItemRepository;
        }

        public async Task CreateReceiptLots()
        {
            if (Environment.EnvironmentName == "Development") return;

            var receiptNoteItems = await _receiptNoteItemRepository.GetAllWithoutReceiptLots();

            foreach (var receiptNoteItem in receiptNoteItems)
            {
                try
                {
                    await _receiptNoteLotsService.CreateLots(receiptNoteItem);
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("ReceiptNoteLots: Não foi possível realizar a criação do lote do item da nota de retorno Id {0}, Exception: {1} {2} {3}", receiptNoteItem.Id, ex.Message, ex.InnerException, ex.StackTrace));
                }
            }
        }
    }
}
