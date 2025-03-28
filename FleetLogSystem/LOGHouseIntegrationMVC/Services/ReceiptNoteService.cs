using LOGHouseSystem.Controllers;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Models;
using LOGHouseSystem.Models.SendEmails;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Org.BouncyCastle.Asn1.Cmp;

namespace LOGHouseSystem.Services
{
    public class ReceiptNoteService : IReceiptNoteService
    {
        public ProductService _productService;
        public ReceiptNoteRepository _receiptNoteRepository;
        private readonly IEmailService _emailService;
        private readonly ILabelBillingRepository _labelBillingRepository;
        private readonly IPositionAndProductService _positionAndProductService;
        private readonly IDevolutionAndReceiptNoteRepository _devolutionAndReceiptNoteRepository;

        public ReceiptNoteService(IEmailService emailService, 
            ILabelBillingRepository labelBillingRepository,
            IPositionAndProductService positionAndProductService,
            IDevolutionAndReceiptNoteRepository devolutionAndReceiptNoteRepository,
            IReceiptNoteItemRepository receiptNoteItemRepository,
            IInventoryMovementRepository inventoryMovementRepository,
            IReceiptNoteLotsService receiptNoteLotsService)
        {
            _emailService = emailService;
            _receiptNoteRepository = new ReceiptNoteRepository();
            _labelBillingRepository = new LabelBillingRepository();
            _positionAndProductService = positionAndProductService;
            _devolutionAndReceiptNoteRepository = devolutionAndReceiptNoteRepository;
            _productService = new ProductService(inventoryMovementRepository, receiptNoteItemRepository, receiptNoteLotsService, positionAndProductService);
        }

        public ResponseDTO ConfirmReceiptNote(int id)
        {
            try
            {
                var note = _receiptNoteRepository.GetById(id);

                if (note == null)
                    return new ResponseDTO(false, "Nota fiscal não encontrada, verifique e tente novamente.");

                _productService.AddStockItemToProduct(id);

                note.Status = Infra.Enums.NoteStatus.AguardandoEnderecamento;
                _receiptNoteRepository.Update(note);

                //_emailService.SendEmail(new EmailData
                //{
                //    EmailBody = $@"A sua nota numero {note.Number} com chave de acesso {note.AccessKey} foi recepcionada, conferida e a mercadoria já se encontra disponível no estoque.",
                //    EmailSubject = $@"Nota {note.Number} recebida com sucesso.",
                //    EmailToId = note.Client.Email,
                //    EmailToName = note.Client.SocialReason
                //}, null, note.Client.Id);

                return new ResponseDTO(true, null);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "Erro ao confirmar recebimento da nota! Erro: " + (ex ?? ex.InnerException).Message);
            }
        }
        public ResponseDTO RejectReceiptNote(int id, string subject, string? message)
        {
            try
            {
                var note = _receiptNoteRepository.GetById(id);

                if (note == null)
                    return new ResponseDTO(false, "Nota fiscal não encontrada, verifique e tente novamente.");

                note.Status = Infra.Enums.NoteStatus.Rejeitada;

                _receiptNoteRepository.Update(note);

                message = $"Número da nota: {note.Number} <br/> <br/> {message}";

                _emailService.ReceiveMessageAndSendEmail(subject, message, null, note.ClientId);

                return new ResponseDTO(true, null);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "Erro ao rejeitar recebimento da nota! Erro: " + (ex ?? ex.InnerException).Message);
            }
        }

        public ResponseDTO ResetReceiptNote(int id)
        {
            try
            {
                var note = _receiptNoteRepository.GetById(id);

                if (note == null)
                    return new ResponseDTO(false, "Nota fiscal não encontrada, verifique e tente novamente.");

                note.Status = Infra.Enums.NoteStatus.Aguardando;

                foreach (var item in note.ReceiptNoteItems)
                {
                    item.QuantityInspection = 0;
                    item.ItemStatus = Infra.Enums.NoteItemStatus.Aguardando;
                }

                _receiptNoteRepository.Update(note);

                return new ResponseDTO(true, null);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "Erro ao resetar contagem da nota! Erro: " + (ex ?? ex.InnerException).Message);
            }
        }

        public List<ReceiptNoteDashboardViewModel> ReceiptNoteToDashboard()
        {
            List<ReceiptNote> receiptNoteList = new List<ReceiptNote>();

            var aguardando = _receiptNoteRepository.GetByStatus(Infra.Enums.NoteStatus.Aguardando);

            var emAndamento = _receiptNoteRepository.GetByStatus(Infra.Enums.NoteStatus.EmAndamento);

            var notaOk = _receiptNoteRepository.GetByStatus(Infra.Enums.NoteStatus.NotaOk);

            var notaDivergente = _receiptNoteRepository.GetByStatus(Infra.Enums.NoteStatus.NotaDivergente);

            receiptNoteList.AddRange(aguardando);
            receiptNoteList.AddRange(emAndamento);
            receiptNoteList.AddRange(notaOk);
            receiptNoteList.AddRange(notaDivergente);

            return receiptNoteList.Select(x => ReceiptNoteToDashboardMapper(x)).ToList();
        }
        public ReceiptNoteDashboardViewModel ReceiptNoteToDashboardMapper(ReceiptNote note)
        {
            return new ReceiptNoteDashboardViewModel()
            {
                Id = note.Id,
                Cnpj = note.Client.Cnpj,
                SocialName = note.Client.SocialReason,
                Status = note.Status,
                ConcludedPercent = getPercent(note),
                AccessKey = note.AccessKey,
                Date = note.EntryDate ?? DateTime.Now,
                InvoiceNumber = note.Number,
                IsDevolution = note.IsDevolution,
            };
        }

        public int getPercent(ReceiptNote note)
        {
            double totalItems = 0;
            double totalitemsCount = 0;

            foreach (var item in note.ReceiptNoteItems)
            {
                totalItems += item.Quantity;
                totalitemsCount += item.QuantityInspection;
            }

            return (int)((totalitemsCount * 100) / totalItems);
        }

        public bool DeleteById(int id) 
        { 
           _devolutionAndReceiptNoteRepository.DeleteByReceiptNoteId(id);

           var result =  _receiptNoteRepository.Delete(id);

            if(result == false)
                throw new Exception("Houve um erro na deleção da nota");

            return result;
        }

        public string GetReceiptNoteNumberByAcessKey(string acessKey)
        {

            if(string.IsNullOrEmpty(acessKey))
                throw new Exception("Ess item não possui a chave de acesso da nota, por favor, fale com o responsável pelo site.");
            
            ReceiptNote noteByAcessKey = _receiptNoteRepository.GetByAcessKey(acessKey);

            string number = noteByAcessKey.Number;

            return number;
        }
    }
}
