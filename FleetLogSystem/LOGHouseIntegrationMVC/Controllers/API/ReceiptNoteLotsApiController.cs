using LOGHouseSystem.Services;
using LOGHouseSystem.Services.HangFire;
using LOGHouseSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptNoteLotsApiController : ControllerBase
    {
        private IReceptNoteLotsRoutine _receptNoteLotsRoutine;
        private IExpeditionOrderService _expeditionOrderService;
        private IReturnInvoiceService _returnInvoiceService;

        public ReceiptNoteLotsApiController(IReceptNoteLotsRoutine receptNoteLotsRoutine,
            IExpeditionOrderService expeditionOrderService,
            IReturnInvoiceService returnInvoiceService)
        {
            _receptNoteLotsRoutine = receptNoteLotsRoutine;
            _expeditionOrderService = expeditionOrderService;
            _returnInvoiceService = returnInvoiceService;
        }


        [HttpGet("GenerateReceiptNoteLots")]
        public async Task GenerateReceiptNoteLots()
        {
            await _receptNoteLotsRoutine.CreateReceiptLots();
        }

        [HttpGet("RemoveLotsAndGenerateReturnNoteByDate")]
        public async Task RemoveLotsAndGenerateReturnNoteByDate()
        {
            try
            {
                var clients = await _expeditionOrderService.GetOrdersIsNoGeneratedReturnInvoiceGroupedByClientAndDate(new DateTime(2024, 2, 29, 23, 59, 59));

                foreach (var client in clients)
                {
                    await _returnInvoiceService.AddReturnInvoiceAndItems(client.Orders);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível criar a nota de retorno, detalhe do erro: " + ex);
            }
        }
    }
}
