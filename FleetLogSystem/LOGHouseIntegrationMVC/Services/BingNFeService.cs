using LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension;
using LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto;
using LOGHouseSystem.Adapters.Extensions.NFeExtension;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Models.SendEmails;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using static LOGHouseSystem.Services.ReceiptNoteLotsService;

namespace LOGHouseSystem.Services
{
    public class BingNFeService : IBlingNFeService
    {
        private IExpeditionOrderRepository _expeditionOrderRepository;
        
        private IClientsRepository _clientsRepository;
        private IReceiptNoteItemRepository _receiptNoteItemRepository;
        private INFeExtension _nFeExtension;
        private IReturnInvoiceRepository _returnInvoiceRepository;
        private IAPIBlingService _aPIBlingService;
        private IReceiptNoteLotsService _receiptNoteLotsService;
        private IReceiptNoteRepository _receiptNoteRepository;
        private IEmailService _emailService;
        private IReturnInvoiceProductInvoicesRepository _returnInvoiceProductInvoicesRepository;

        public BingNFeService(IExpeditionOrderRepository expeditionOrderRepository,
            IClientsRepository clientsRepository, 
            IReceiptNoteItemRepository receiptNoteItemRepository, 
            INFeExtension nFeExtension, 
            IReturnInvoiceRepository returnInvoiceRepository, 
            IAPIBlingService aPIBlingService, 
            IReceiptNoteLotsService receiptNoteLotsService, 
            IReceiptNoteRepository receiptNoteRepository,
            IEmailService emailService,
            IReturnInvoiceProductInvoicesRepository returnInvoiceProductInvoicesRepository)
        {
            _expeditionOrderRepository = expeditionOrderRepository;            
            _clientsRepository = clientsRepository;
            _receiptNoteItemRepository = receiptNoteItemRepository;
            _nFeExtension = nFeExtension;
            _returnInvoiceRepository = returnInvoiceRepository;
            _aPIBlingService = aPIBlingService;
            _receiptNoteLotsService = receiptNoteLotsService;
            _receiptNoteRepository = receiptNoteRepository;
            _emailService = emailService;
            _returnInvoiceProductInvoicesRepository = returnInvoiceProductInvoicesRepository;
        }

        public async Task SendNfeFromReturnInvoice(int returnInvoiceId, int clientIdEmit)
        {
            // Take notes to emit
            ReturnInvoice returnInvoice = await _returnInvoiceRepository.GetByIdAsync(returnInvoiceId);

            await SendNfeFromReturnInvoice(returnInvoice, clientIdEmit);
        }

        public async Task SendNfeFromReturnInvoice(ReturnInvoice returnInvoice, int clientIdEmit)
        {            
            List<string> productsEans = new List<string>();
            

            if (returnInvoice == null)
            {
                throw new Exception("Nota de retorno não encontrada.");
            }

            var receiptNotesIds = returnInvoice.ReturnInvoiceItems.Select(e => Convert.ToInt32(e.ReceiptNoteItemId));

            List<ReceiptNote> receiptNotes = await _receiptNoteRepository.GetAllById(receiptNotesIds);

            // Take only the access token
            var accessTokenList = receiptNotes.Select(e => e.AccessKey).Distinct().ToList();

            // Get invoice products
            List<ReturnInvoiceProductInvoices> productsInvoice = await _returnInvoiceProductInvoicesRepository.GetByReturnInvoiceId(returnInvoice.Id);

            // Get Nfe Proc
            List<NfeProc> nfeProcs = await GetXmlFilesFromAccessKeyList(accessTokenList, productsInvoice);

            // Get Nfe
            var xmlNoteClient = nfeProcs.FirstOrDefault();

            // take client
            var client = await _clientsRepository.GetByIdAsync(Convert.ToInt32(returnInvoice.ClientId));

            // get last invoice received from client to get client data
            NfeProc lastClientNfeProc = await GetLastClientNfeProc(returnInvoice.ClientId);           

            ValidateProducts(nfeProcs, returnInvoice.ReturnInvoiceItems, receiptNotes);

            var nfe = new BlingNfeRequestDto()
            {
                Tipo = 1, // 0 - Entrada | 1 - Saida
                DataOperacao = DateTime.Now.ToString("yyyy-MM-dd"),
                Contato = new BlingNfeContatoRequestDto()
                {
                    // indicadorContribuinteICMS: "NaoContribuinte",
                    Contribuinte = 1,
                    Nome = lastClientNfeProc.NFe.InfNFe.Emit.XNome,
                    TipoPessoa = "J",
                    NumeroDocumento = lastClientNfeProc.NFe.InfNFe.Emit.CNPJ,
                    IE = lastClientNfeProc.NFe.InfNFe.Emit.IE,
                    Endereco = new BlingNfeEnderecoRequestDto()
                    {
                        Endereco = lastClientNfeProc.NFe.InfNFe.Emit.Ender.XLgr,
                        Numero = lastClientNfeProc.NFe.InfNFe.Emit.Ender.Nro,
                        Complemento = lastClientNfeProc.NFe.InfNFe.Emit.Ender.XCpl,
                        Bairro = lastClientNfeProc.NFe.InfNFe.Emit.Ender.XBairro,
                        Cep = lastClientNfeProc.NFe.InfNFe.Emit.Ender.CEP,
                        UF = lastClientNfeProc.NFe.InfNFe.Emit.Ender.UF,
                        Municio = lastClientNfeProc.NFe.InfNFe.Emit.Ender.XMun,
                        Pais = lastClientNfeProc.NFe.InfNFe.Emit.Ender.xPais
                    }
                },
                Finalidade = 1,
                Serie = 2,
                Seguro = 0,
                Despesas = 0,
                Desconto = 0,
                Observacoes = $"Retorno conforme as notas referenciadas: {string.Join(", ", accessTokenList)}",
                Items = new List<BlingNfeItensRequestDto>()
            };

            foreach (var item in returnInvoice.ReturnInvoiceItems)
            {

                // get receipt note item
                // var receiptNoteItem = await _receiptNoteItemRepository.GetByIdAsync(Convert.ToInt32(item.ReceiptNoteItemId));

                var receiptNote = await _receiptNoteRepository.GetByIdAsync(Convert.ToInt32(item.ReceiptNoteItemId));

                Det det = GetDetProductFromNfeProcs(nfeProcs, receiptNotes, Convert.ToInt32(item.ReceiptNoteItemId), item.Product);

                var receiptNoteItem = receiptNote.ReceiptNoteItems.Where(e => e.Ean == item.Ean).FirstOrDefault();

                var itemNfe = new BlingNfeItensRequestDto()
                {
                    Cest = det.Prod.CEST,
                    Codigo = item.ProductId.ToString(),
                    Descricao = receiptNoteItem.Description,
                    PesoBruto = 0,
                    PesoLiquido = 0,
                    Quantidade = Convert.ToInt32(item.Quantity),
                    //Valor = Math.Round(Convert.ToDecimal(receiptNoteItem.Value) * Convert.ToInt32(item.Quantity), 2),
                    Valor = Convert.ToDecimal(item.Value),
                    Unidade = det.Prod.UTRIB,
                    Tipo = "P",                    
                    CodigoServico = "",
                    InformacoesAdicionais = "",
                    NumeroPedidoCompra = det.Prod.CProd,
                    ClassificacaoFiscal = det.Prod.NCM
                };

                if (det.Imposto.ICMS.ICMS00 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMS00.Orig;
                }
                else if (det.Imposto.ICMS.ICMS10 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMS10.Orig;
                }
                else if (det.Imposto.ICMS.ICMS20 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMS20.Orig;
                }
                else if (det.Imposto.ICMS.ICMS30 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMS30.Orig;
                }
                else if (det.Imposto.ICMS.ICMS40 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMS40.Orig;
                }
                else if (det.Imposto.ICMS.ICMS41 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMS41.Orig;
                }
                else if (det.Imposto.ICMS.ICMS50 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMS50.Orig;
                }
                else if (det.Imposto.ICMS.ICMS51 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMS51.Orig;
                }
                else if (det.Imposto.ICMS.ICMS60 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMS60.Orig;
                }
                else if (det.Imposto.ICMS.ICMS70 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMS70.Orig;
                }
                else if (det.Imposto.ICMS.ICMS90 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMS90.Orig;
                }
                else if (det.Imposto.ICMS.ICMSSN101 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMSSN101.Orig;
                }
                else if (det.Imposto.ICMS.ICMSSN101 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMSSN102.Orig;
                }
                else if (det.Imposto.ICMS.ICMSSN102 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMSSN102.Orig;
                }
                else if (det.Imposto.ICMS.ICMSSN103 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMSSN103.Orig;
                }
                else if (det.Imposto.ICMS.ICMSSN201 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMSSN201.Orig;
                }
                else if (det.Imposto.ICMS.ICMSSN202 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMSSN202.Orig;
                }
                else if (det.Imposto.ICMS.ICMSSN203 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMSSN203.Orig;
                }
                else if (det.Imposto.ICMS.ICMSSN400 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMSSN400.Orig;
                }
                else if (det.Imposto.ICMS.ICMSSN500 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMSSN500.Orig;
                }
                else if (det.Imposto.ICMS.ICMSSN900 != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMSSN900.Orig;
                }
                else if (det.Imposto.ICMS.ICMSST != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMSST.Orig;
                }
                else if (det.Imposto.ICMS.ICMSPART != null)
                {
                    itemNfe.Origem = det.Imposto.ICMS.ICMSPART.Orig;
                }

                nfe.Items.Add(itemNfe);
            }

            var environmentResponse = await _aPIBlingService.GetEnvironmentOperation(Environment.BlingV3EnvironmentOperationDescription, clientIdEmit);

            BlingNfeErrorResponseDto responseError = null;

            if (environmentResponse.IsSuccessStatusCode)
            {

                BlingNfeNaturezaOperacaoResponseDto operationDto = JsonConvert.DeserializeObject<BlingNfeNaturezaOperacaoResponseDto>(environmentResponse.Content);
                var operation = operationDto.Data.FirstOrDefault();

                if (operation != null)
                {
                    nfe.NaturezaOperacao = new BlingNfeNaturezaOperacaoRequestDto()
                    {
                        Id = operation.Id
                    };

                    RestResponse responseSendNoteContent;

                    if (!string.IsNullOrEmpty(returnInvoice.ExternalId))
                    {
                        responseSendNoteContent  = await _aPIBlingService.UpdateNfe(nfe, Convert.ToInt64(returnInvoice.ExternalId), clientIdEmit);
                    }
                    else
                    {
                        responseSendNoteContent = await _aPIBlingService.SendNfe(nfe, clientIdEmit);
                    }                    

                    if (responseSendNoteContent.IsSuccessStatusCode)
                    {
                        BlingNfeResponseDto responseSendNote = JsonConvert.DeserializeObject<BlingNfeResponseDto>(responseSendNoteContent.Content);
                        
                        var responseConfirmNote = await _aPIBlingService.ConfirmNfe(responseSendNote.Data.Id, clientIdEmit);
                        returnInvoice.ExternalId = responseSendNote.Data.Id.ToString();

                        if (responseConfirmNote.IsSuccessStatusCode)
                        {
                            BlingNfeConfirmResponseDto responseContent = JsonConvert.DeserializeObject<BlingNfeConfirmResponseDto>(responseConfirmNote.Content);
                            returnInvoice.Status = ReturnInvoiceStatus.Aprovada;

                            try
                            {
                                if (responseContent.Data.Xml != null)
                                {
                                    RestResponse getNfe = await _aPIBlingService.GetNFe(returnInvoice.ExternalId, clientIdEmit);

                                    BlingV3GetInvoiceResponse blingV3GetInvoiceResponse = JsonConvert.DeserializeObject<BlingV3GetInvoiceResponse>(getNfe.Content);

                                    var xml = _nFeExtension.DeserializeFromXml<BingRetConsReciNFeResponseDto>(responseContent.Data.Xml);

                                    returnInvoice.InvoiceAccessKey = xml.ProtNFe.InfProt.ChNFe;
                                    returnInvoice.InvoiceNumber = blingV3GetInvoiceResponse.data.numero;
                                    returnInvoice.Xml = blingV3GetInvoiceResponse.data.xml;
                                    returnInvoice.LinkDanfe = blingV3GetInvoiceResponse.data.linkDanfe;
                                    returnInvoice.LinkPdf = blingV3GetInvoiceResponse.data.linkPDF;

                                    Log.Info("Nota enviada com sucesso.");
                                }
                                else
                                {
                                    returnInvoice.Rejection = responseConfirmNote.Content;
                                    returnInvoice.Status = ReturnInvoiceStatus.Rejeitada;
                                    Log.Error("Nota enviada com erro.", responseConfirmNote.Content);
                                }
                            }
                            catch (Exception ex)
                            {
                                returnInvoice.Rejection = responseConfirmNote.Content;
                                returnInvoice.Status = ReturnInvoiceStatus.Rejeitada;
                                Log.Error("Nota enviada com erro.", ex.Message);                                
                            }
                            
                        }
                        else
                        {
                            responseError = JsonConvert.DeserializeObject<BlingNfeErrorResponseDto>(responseConfirmNote.Content);
                            Log.Error("Nota enviada com erro.", responseConfirmNote.Content);
                        }
                    }
                    else
                    {
                        responseError = JsonConvert.DeserializeObject<BlingNfeErrorResponseDto>("");
                        Log.Error("Nota enviada com erro.", "");
                    }
                }
                else
                {
                    returnInvoice.Rejection = "Nota não enviada, nenhuma natureza de operação encontrada.";
                    Log.Error("Nota não enviada, nenhuma natureza de operação encontrada.", environmentResponse.Content);
                }                
            }
            else
            {
                returnInvoice.Rejection = "Nota não enviada, nenhuma natureza de operação encontrada.";
                Log.Error("Nota não enviada, nenhuma natureza de operação encontrada.", environmentResponse.Content);
            }

            
            if (responseError != null)
            {
                returnInvoice.Status = ReturnInvoiceStatus.Rejeitada;

                var message = responseError.Error.Message;

                if (responseError.Error.Fields != null && responseError.Error.Fields.Count > 0)
                {

                    foreach (var item in responseError.Error.Fields)
                    {
                        message += ". " + item.Msg;

                        foreach (var item2 in item.Collection)
                        {
                            message += ". " + item2.Msg;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(returnInvoice.Rejection))
                {
                    _emailService.SendEmail(new EmailData
                    {
                        EmailBody = $@"Não foi possivel emitir a nota de retorno para o cliente Cliente {client.SocialReason} - {client.Cnpj}. 
                            <br> Confira o painel de Notas de Retorno e o painel do Bling para mais informações. 
                            <br> Nota de Retorno: {returnInvoice.Id} 
                            <br> Notas referenciadas: {string.Join(", ", accessTokenList)}
                            <br> Data emissão: {DateTime.Now.ToString("yyyy-MM-dd")}",
                        EmailSubject = $"Nota fiscal de retorno não emitida. Cliente {client.SocialReason} - {client.Cnpj}.",
                        EmailToId = Environment.BlingV3EmailRejectionNotification,
                        EmailToName = Environment.BlingV3EmailRejectionNotification
                    }, null, clientIdEmit);
                }

                returnInvoice.Rejection = message;
            }            

            await _returnInvoiceRepository.UpdateAsync(returnInvoice);
        }

        private async Task<NfeProc> GetLastClientNfeProc(int? clientId)
        {
            ReceiptNote lastReceiptNote = await _receiptNoteRepository.GetLastReceiptNoteAsync(Convert.ToInt32(clientId));

            List<NfeProc> lastReceiptNoteProc = await GetXmlFilesFromAccessKeyList(new List<string>(){
                lastReceiptNote.AccessKey
            });

            if (lastReceiptNoteProc.Count <= 0)
            {
                throw new Exception("É necessário ter ao menos uma nota de entrada valida para podermos buscar os dados do destinatário.");
            }

            return lastReceiptNoteProc[0];
        }

        private void ValidateProducts(List<NfeProc> nfeProcs, List<ReturnInvoiceItem> items, List<ReceiptNote> receiptNotes)
        {
            string msg = "";
            foreach (var item in items)
            {

                var det = GetDetProductFromNfeProcs(nfeProcs, receiptNotes, Convert.ToInt32(item.ReceiptNoteItemId), item.Product);

                if (det == null)
                {
                    msg = $"{msg} {item.Name} - {item.Ean} - {item.Description}<br>";
                }
            }

            if (!string.IsNullOrEmpty(msg))
            {
                msg = $"Produtos não encontrados na base: <br> {msg} É necessário anexar o XML com esses produtos.";
                throw new Exception(msg);
            }
        }
        private Det GetDetProductFromNfeProcs(List<NfeProc> nfeProcs, List<ReceiptNote> receiptNotes, int receiptNoteId, Product product)
        {

            // get receipt note
            var receiptNote = receiptNotes.Where(e => e.Id == receiptNoteId).FirstOrDefault();

            // get xml
            var xmlNote = nfeProcs.Where(e => e.ProtNFe.InfProt.ChNFe == receiptNote.AccessKey).FirstOrDefault();

            Det det = null;

            if (xmlNote == null)
            {
                xmlNote = nfeProcs.Where(xml => xml.NFe.InfNFe.Det.Where((e) => SearchProductInNf(e, product)).Any()).FirstOrDefault();
            }

            if (xmlNote == null)
            {
                return null;
            }
            det = xmlNote.NFe.InfNFe.Det.Where((e) => SearchProductInNf(e, product)).FirstOrDefault();

            return det;
        }

        private bool SearchProductInNf(Det e, Product product)
        {
            string ean = "";

            if (e.Prod.CEAN.ToString() == "SEM GTIN")
            {
                ean = e.Prod.CProd;
            }
            else
            {
                var multipleEan = e.Prod.CEAN.ToString().Split("|");

                ean = multipleEan[0];
            }

            if (e.Prod.CProd == product.Code || ean == product.Ean)
            {
                return true;
            }

            return false;
        }
           
        private async Task<List<NfeProc>> GetXmlFilesFromAccessKeyList(List<string> accessCode, List<ReturnInvoiceProductInvoices> productsInvoice = null)
        {
            var returnContent = new List<NfeProc>();

            foreach (var accessCodeItem in accessCode)
            {

                var xmlPath = $"{Environment.XmlUploadPath}/{accessCodeItem}.xml";

                if (!File.Exists(xmlPath))
                {
                    continue;
                }

                NfeProc nfe = await _nFeExtension.GetNfeByPathAsync(xmlPath);

                returnContent.Add(nfe);
            }

            if (productsInvoice != null)
            {
                foreach (var invoice in productsInvoice)
                {

                    if (!File.Exists(invoice.XmlPath))
                    {
                        throw new Exception($"O arquivo {invoice.XmlPath} não foi encontrado.");
                    }

                    NfeProc nfe = await _nFeExtension.GetNfeByPathAsync(invoice.XmlPath);

                    returnContent.Add(nfe);
                }
            }
            

            return returnContent;
        }
    }
}
