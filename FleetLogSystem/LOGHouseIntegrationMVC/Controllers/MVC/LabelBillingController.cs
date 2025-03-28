using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.MVC
{
    public class LabelBillingController : Controller
    {
        private readonly ILabelBillingRepository _labelBillingRepository;
        private readonly IReceiptNoteRepository _receiptNoteRepository;
        private readonly IEmailService _emailService;

        public LabelBillingController(ILabelBillingRepository labelBillingRepository, IReceiptNoteRepository receiptNoteRepository, IEmailService emailService)
        {
            _labelBillingRepository = labelBillingRepository;
            _receiptNoteRepository = receiptNoteRepository;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<LabelBilling> Add([FromBody] List<ReceiptNoteItemTransferViewModel> itens)
        {
            try
            {
                //var values = JsonConvert.DeserializeObject<List<ReceiptNoteItemTransferViewModel>>(itens);

                decimal totalQuantidade = itens.Sum(x => decimal.Parse(x.Quantidade));

                ReceiptNote note = _receiptNoteRepository.GetById(int.Parse(itens[0].NoteId));
                string message = $"Id da Nota: {note.Id} | Número da Nota: {note.Number} | <br><br>";
                decimal quantity = 0;

                foreach (var item in itens)
                {
                    message += $"Código do Produto: {item.Ean}, Quantidade: {item.Quantidade} unidades | ";
                    quantity += Convert.ToDecimal(item.Quantidade);
                }

                if (itens.Count > 0)
                {
                    message = message.Substring(0, message.Length - 3);
                }

                LabelBilling labelBilling = new LabelBilling()
                {
                    ClientId = note.ClientId,
                    Description = message,
                    Value = totalQuantidade,
                    Status = Infra.Enums.LabelBillingEnum.Aguardando,
                    ReceiptNoteId = note.Id
                };

                message = message + $"<br> Total: {totalQuantidade} unidades <br>";

                _emailService.ReceiveMessageAndSendEmail("Cobrança de Etiquetas", "Estamos enviando esse e-mail com o intuito de avisa-lo que recepcionamos alguns produtos na data de hoje fora do padrão recebimento, com isso, será necessário etiquetar esses produtos e isso irá gerar uma cobrança excedente em sua fatura.<br><br> "  + message, null, note.ClientId);

                await _labelBillingRepository.CreateAsync(labelBilling);

                return labelBilling;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
