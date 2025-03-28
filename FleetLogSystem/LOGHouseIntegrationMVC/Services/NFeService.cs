using DocumentFormat.OpenXml.Wordprocessing;
using LOGHouseSystem.Adapters.Extensions.NFeExtension;
using LOGHouseSystem.Controllers;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using Org.BouncyCastle.Asn1.Ocsp;

namespace LOGHouseSystem.Services
{
    public class NFeService : INFeService
    {
        public INFeExtension _nfeExtension;
        public ReceiptNoteRepository _receiptNoteRepository;
        public ClientsRepository _clientRepository;
        private readonly IInvoiceService _invoiceService;
        private IExpeditionOrderRepository _expeditionOrderRepository;
        private IProductRepository _productRepository;
        private IClientsRepository _clientsRepository;

        public NFeService(INFeExtension nFeExtension, IInvoiceService invoiceService, IExpeditionOrderRepository expeditionOrderRepository, IProductRepository productRepository, IClientsRepository clientsRepository)
        {
            _nfeExtension = nFeExtension;
            _receiptNoteRepository = new ReceiptNoteRepository();
            _clientRepository = new ClientsRepository();
            _invoiceService = invoiceService;
            _expeditionOrderRepository = expeditionOrderRepository;
            _productRepository = productRepository;
            _clientsRepository = clientsRepository;
        }

        //Used in BlingMethod, so Invoice must be used
        public async Task<ResponseDTO> ImportNfeAsync(string fileText, int statusDevolution = 0, int isScheduling = 0, bool validateStock = false)
        {
            (var invoice, var responseDto, var items, var proc) = GenerateInvoiceBase(fileText, statusDevolution, isScheduling, validateStock);

            if (responseDto != null)
            {
                return responseDto;
            }

            var existingNote = await _invoiceService.GetByAcessKeyAsync(proc.ProtNFe.InfProt.ChNFe.ToString());

            if (existingNote != null)
                return new ResponseDTO(false, $"Chave de acesso já importada. Verifique e tente novamente.");

            Invoice invoiceInstnaced = new Invoice();

            invoiceInstnaced.Id = invoice.Id;
            invoiceInstnaced.Number = invoice.Number;
            invoiceInstnaced.SerialNumber = invoice.SerialNumber;
            invoiceInstnaced.AccessKey = invoice.AccessKey;
            invoiceInstnaced.EmitDocument = invoice.EmitDocument;
            invoiceInstnaced.DestDocument = invoice.DestDocument;
            invoiceInstnaced.EntryDate = invoice.EntryDate;
            invoiceInstnaced.IssueDate = invoice.IssueDate;
            invoiceInstnaced.Status = invoice.Status;
            invoiceInstnaced.TotalInvoiceValue = invoice.TotalInvoiceValue;
            

            List<InvoiceItem> invoiceItems = new List<InvoiceItem>();
            foreach (var item in items)
            {
                InvoiceItem itemConverted = new InvoiceItem()
                {
                    Id = item.Id,
                    Code = item.Code,
                    Description = item.Description,
                    Ean = item.Ean,
                    Quantity = item.Quantity,
                    QuantityInspection = item.QuantityInspection,
                    Value = item.Value,
                    ItemStatus = item.ItemStatus,
                    
                };
                
                invoiceItems.Add(itemConverted);
            }

            ExpeditionOrder order = await _expeditionOrderRepository.GetOrderByInvoiceAccessKeyAsync(invoice.AccessKey);

            if (order != null)
            {
                invoiceInstnaced.ExpeditionOrderId = order.Id;
            }
            
            invoiceInstnaced.InvoiceItems = invoiceItems;
            

            await _invoiceService.AddAsync(invoiceInstnaced);

            await SaveFile(invoiceInstnaced.AccessKey, fileText);

            return new ResponseDTO(true, null, proc, 0);
        }

        //Used in the upload files method, so you must use receiptNote
        public ResponseDTO GenerateNfeProc(string fileText, int statusDevolution = 0, int isScheduling = 0, bool validateStock = false, bool validarNota = true)
        {
            (var invoice, var responseDto, var items, var nfe) = GenerateInvoiceBase(fileText, statusDevolution, isScheduling, validateStock, validarNota);

            if (responseDto != null)
            {
                return responseDto;
            }

            ReceiptNote receiptNote;            

            var client = _clientRepository.FindByCnpj(invoice.EmitDocument);

            receiptNote = new ReceiptNote()
            {
                Id = invoice.Id,
                Number = invoice.Number,
                SerialNumber = invoice.SerialNumber,
                AccessKey = invoice.AccessKey,
                EmitDocument = invoice.EmitDocument,
                DestDocument = invoice.DestDocument,
                EntryDate = invoice.EntryDate,
                IssueDate = invoice.IssueDate,
                Status = invoice.Status,
                TotalInvoiceValue = nfe.NFe.InfNFe.Total.ICMSTot.vNF
            };

            List<ReceiptNoteItem> noteItems = new List<ReceiptNoteItem>();
            foreach (var item in items)
            {
                ReceiptNoteItem itemConverted = new ReceiptNoteItem()
                {
                    Id = item.Id,
                    Code = item.Code,
                    Description = item.Description,
                    Ean = item.Ean,
                    Quantity = item.Quantity,
                    QuantityInspection = item.QuantityInspection,
                    Value = item.Value,
                    ItemStatus = item.ItemStatus,
                };

                noteItems.Add(itemConverted);
            }

            receiptNote.ReceiptNoteItems = noteItems;

            if (client != null)
            {
                receiptNote.ClientId = client.Id;
            }            

            return new ResponseDTO(true, null, nfe);
        }

        //Used in the upload files method, so you must use receiptNote
        public ResponseDTO ImportNfe(string fileText, int statusDevolution = 0, int isScheduling = 0, bool validateStock = false)
        {
            (var invoice, var responseDto, var items, var nfe) = GenerateInvoiceBase(fileText, statusDevolution, isScheduling, validateStock);

            if (responseDto != null)
            {
                return responseDto;
            }

            ReceiptNote receiptNote;

            var existingNote = _receiptNoteRepository.GetByAcessKey(nfe.ProtNFe.InfProt.ChNFe.ToString());

            var client = _clientRepository.FindByCnpj(invoice.EmitDocument);

            if (client == null)
                return (new ResponseDTO(false, "O emissor da nota não foi encontrado na base de dados. Realize o cadastro do mesmo e tente novamente."));

            if (existingNote != null)
                return new ResponseDTO(false, $"Chave de acesso já importada. Verifique e tente novamente.");

            if (statusDevolution > 0)
            {
                receiptNote = new ReceiptNote()
                {
                    Id = invoice.Id,
                    Number = invoice.Number,
                    SerialNumber = invoice.SerialNumber,
                    AccessKey = invoice.AccessKey,
                    EmitDocument = invoice.EmitDocument,
                    DestDocument = invoice.DestDocument,
                    EntryDate = invoice.EntryDate,
                    IssueDate = invoice.IssueDate,
                    Status = invoice.Status,
                    TotalInvoiceValue = nfe.NFe.InfNFe.Total.ICMSTot.vNF,
                    IsDevolution = YesOrNo.Yes
                };
            }
            else
            {
                receiptNote = new ReceiptNote()
                {
                    Id = invoice.Id,
                    Number = invoice.Number,
                    SerialNumber = invoice.SerialNumber,
                    AccessKey = invoice.AccessKey,
                    EmitDocument = invoice.EmitDocument,
                    DestDocument = invoice.DestDocument,
                    EntryDate = invoice.EntryDate,
                    IssueDate = invoice.IssueDate,
                    Status = invoice.Status,
                    TotalInvoiceValue = nfe.NFe.InfNFe.Total.ICMSTot.vNF
                };
            }

            List<ReceiptNoteItem> noteItems = new List<ReceiptNoteItem>();
            foreach (var item in items)
            {
                ReceiptNoteItem itemConverted = new ReceiptNoteItem()
                {
                    Id = item.Id,
                    Code = item.Code,
                    Description = item.Description,
                    Ean = item.Ean,
                    Quantity = item.Quantity,
                    QuantityInspection = item.QuantityInspection,
                    Value = item.Value,
                    ItemStatus = item.ItemStatus,
                };

                noteItems.Add(itemConverted);
            }

            receiptNote.ReceiptNoteItems = noteItems;

            receiptNote.ClientId = client.Id;

           ReceiptNote note =  _receiptNoteRepository.Add(receiptNote);

            SaveFile(invoice.AccessKey, fileText).Wait();

            return new ResponseDTO(true, null, null, note.Id);
        }

        public (InvoiceBase, ResponseDTO, List<InvoiceItemBase>, NfeProc) GenerateInvoiceBase(string fileText, int statusDevolution, int isScheduling = 0, bool validateStock = false, bool validarNota = true)
        {

            var nfe = _nfeExtension.DeserializeNFe(fileText);

            if (nfe == null)
                return (null, new ResponseDTO(false, "Erro na estrutura do arquivo, verifique e tente novamente."), null, null);

            if (nfe.ProtNFe == null && validarNota)
                return (null, new ResponseDTO(false, "Erro na estrutura do arquivo, não foi encontrado protNfe no XML, verifique e tente novamente."), null, null);

            var client =  _clientsRepository.FindByCnpj(nfe.NFe.InfNFe.Emit.CNPJ);
            if(client == null && validarNota)
                return (null, new ResponseDTO(false, $"CNPJ do emissor não encontrado na base de clientes. Doc na nota: {nfe.NFe.InfNFe.Emit.CNPJ}"), null, null);

            if (isScheduling == 0 && validarNota)
            {
                foreach (var item in nfe.NFe.InfNFe.Det)
                {
                    Product product;
                    if (item.Prod.CEAN.Contains("|"))
                    {
                        var mutipleEan = item.Prod.CEAN.ToString().Split('|');
                        product = _productRepository.GetByEan(mutipleEan[0], client.Id);

                    }
                    else if (item.Prod.CEAN != "SEM GTIN")
                    {
                        product = _productRepository.GetByEan(item.Prod.CEAN, client.Id);
                    }
                    else
                    {
                        product = _productRepository.GetByCode(item.Prod.CProd, client.Id);
                    }

                    if (product == null)
                    {
                        string message = $"O produto {item.Prod.XProd} não consta em nossa base de dados";
                        return (null, new ResponseDTO(false, message), null, null);
                    }
                    else
                    {
                        if (validateStock && validarNota)
                        {
                            if (product.StockQuantity < item.Prod.QCom)
                            {
                                string message = $"Ops! Não foi possível inserir esse produto, {item.Prod.XProd} possui apenas {product.StockQuantity} em nosso estoque";
                                return (null, new ResponseDTO(false, message), null, null);
                            }
                        }
                    }
                }
            }


            var nfeItems = nfe.NFe.InfNFe.Det.Select(x => {

                string ean = "";


                if (x.Prod.CEAN.ToString() == "SEM GTIN")
                {
                    ean = x.Prod.CProd;
                }
                else
                {
                    var multipleEan = x.Prod.CEAN.ToString().Split("|");

                    ean = multipleEan[0];
                }


                return new InvoiceItemBase
                {
                    Ean = ean,
                    Description = x.Prod.XProd,
                    Code = x.Prod.CProd,
                    Quantity = x.Prod.QCom,
                    Value = Convert.ToDecimal(x.Prod.VUnCom),
                    ItemStatus = NoteItemStatus.Aguardando
                };

            }).ToList();


            InvoiceBase invoiceBase = new InvoiceBase
            {
                EmitDocument = nfe.NFe.InfNFe.Emit.CNPJ.ToString(),
                AccessKey = nfe.ProtNFe.InfProt.ChNFe.ToString(),
                Number = nfe.NFe.InfNFe.Ide.NNF.ToString(),
                SerialNumber = nfe.NFe.InfNFe.Ide.Serie.ToString(),
                IssueDate = nfe.NFe.InfNFe.Ide.DhEmi,
                TotalInvoiceValue = nfe.NFe.InfNFe.Total.ICMSTot.vNF,
                Status = NoteStatus.Aguardando,
            };


            if (nfe.NFe.InfNFe.Dest.CNPJ != null)
            {
                invoiceBase.DestDocument = nfe.NFe.InfNFe.Dest.CNPJ.ToString();
            }
            else if (nfe.NFe.InfNFe.Dest.CPF != null)
            {
                invoiceBase.DestDocument = nfe.NFe.InfNFe.Dest.CPF.ToString();
            }
            else
            {
                invoiceBase.DestDocument = nfe.NFe.InfNFe.Dest.IdEstrangeiro.ToString();
            }

            return (invoiceBase, null, nfeItems, nfe);
        }

        public async Task SaveFile(string accessKey, string xml)
        {
            var xmlPath = $"{Environment.XmlUploadPath}/{accessKey}.xml";

            if (File.Exists(xmlPath))
            {
                File.Delete(xmlPath);
            }

            await File.WriteAllTextAsync(xmlPath, xml);
        }

        public ResponseDTO ValidatingXmlData(string fileText, int statusDevolution, int isScheduling = 0)
        {
            (var invoice, var responseDto, var items, var nfe) = GenerateInvoiceBase(fileText, statusDevolution, isScheduling);

            if (responseDto != null)
            {
                return responseDto;
            }

            if (nfe.NFe.InfNFe.Dest.CNPJ != Environment.XmlCnpjLogHouse)
                return new ResponseDTO(false, $"CNPJ Destinatario diferente do CNPJ da Log House {Environment.XmlCnpjLogHouse}");

            var clientLogged = _clientRepository.GetByUserLoged();

            if (clientLogged != null)
            {
                if (clientLogged.Cnpj != invoice.EmitDocument)
                    return new ResponseDTO(false, $"CNPJ do emitente é diferente do CNPJ do cliente autenticado. CNPJ da Nota {invoice.EmitDocument} - CNPJ do cliente autenticado no sistema {clientLogged.Cnpj}");
            }

            var aggroupedItens = items.GroupBy(e => e.Code);

            var duplicatedCodes = new List<string>();

            foreach (var item in aggroupedItens)
            {
                if (item.Count() > 1)
                {
                    duplicatedCodes.Add(item.Key);
                }
            }

            if (duplicatedCodes.Count > 0)
                return new ResponseDTO(false, $"A nota fiscal possui itens com códigos duplicados: {string.Join(',', duplicatedCodes)}");

            var aggroupedEanItens = items.GroupBy(e => e.Ean);

            var duplicatedEans = new List<string>();

            foreach (var item in aggroupedItens)
            {
                if (item.Count() > 1)
                {
                    duplicatedEans.Add(item.Key);
                }
            }

            if (duplicatedCodes.Count > 0)
                return new ResponseDTO(false, $"A nota fiscal possui itens com eans duplicados: {string.Join(',', duplicatedEans)}");



            var client = _clientRepository.FindByCnpj(invoice.EmitDocument);

            if (client == null)
                return (new ResponseDTO(false, "O emissor da nota não foi encontrado na base de dados. Realize o cadastro do mesmo e tente novamente."));

            var invalidsCfops = new List<string>();

            foreach (var item in nfe.NFe.InfNFe.Det)
            {
                if (!(item.Prod.CFOP.EndsWith("949") || item.Prod.CFOP.EndsWith("923")))
                {
                    invalidsCfops.Add(item.Prod.CProd);
                }
            }

            if (invalidsCfops.Count > 0)
                return new ResponseDTO(false, $"A nota fiscal possui itens com CFOPs inválidos: {string.Join(',', invalidsCfops)}");

            return new ResponseDTO(true, $"");
        }
    }
}