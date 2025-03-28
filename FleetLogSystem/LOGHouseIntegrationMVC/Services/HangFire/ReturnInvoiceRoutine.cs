using LOGHouseSystem.Models;
using LOGHouseSystem.Services.HangFire.Interface;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services.HangFire
{
    public class ReturnInvoiceRoutine : IReturnInvoiceRoutine
    {
        private readonly IExpeditionOrderService _expeditionOrderService;
        private readonly IReturnInvoiceService _returnInvoiceService;

        public ReturnInvoiceRoutine(IExpeditionOrderService expeditionOrderService, IReturnInvoiceService returnInvoiceService)
        {
            _expeditionOrderService = expeditionOrderService;
            _returnInvoiceService = returnInvoiceService;
        }

        public async Task ReturnInvoiceRoutineMethod()
        {
            if (Environment.EnvironmentName == "Development") return;

            try
            {
                var clients = await _expeditionOrderService.GetOrdersIsNoGeneratedReturnInvoiceGroupedByClient();

                foreach(var client in clients)
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
