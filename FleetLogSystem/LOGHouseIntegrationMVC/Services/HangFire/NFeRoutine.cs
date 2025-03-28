using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using System;
using System.Linq;

namespace LOGHouseSystem.Services.HangFire
{
    public class NFeRoutine : INFeRoutine
    {
        private IExpeditionOrderRepository _expeditionOrderRepository;
        private IReturnInvoiceService _returnInvoiceService;
        private IBlingNFeService _blingNFeService;
        private IReturnInvoiceRepository _returnInvoiceRepository;        

        public NFeRoutine(IExpeditionOrderRepository expeditionOrderRepository, IReturnInvoiceService returnInvoiceService, IBlingNFeService blingNFeService, IReturnInvoiceRepository returnInvoiceRepository)
        {
            _expeditionOrderRepository = expeditionOrderRepository;
            _returnInvoiceService = returnInvoiceService;
            _blingNFeService = blingNFeService;
            _returnInvoiceRepository = returnInvoiceRepository;
        }

        public async Task Routine()
        {
            if (Environment.EnvironmentName == "Development") return;
            List<ReturnInvoice> returnInvoices = await _returnInvoiceRepository.GetNotSendedInvoices();

            foreach (var invoice in returnInvoices)
            {
                await _blingNFeService.SendNfeFromReturnInvoice(invoice, Convert.ToInt32(Environment.ClientIdLogHouse));
            }            
        }
    }
}
