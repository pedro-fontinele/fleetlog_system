using LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Dto;
using LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Responses;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Models.API.Responses;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.Services.Smartgo;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LOGHouseSystem.Controllers.MVC
{
    public class ImportSmartGoStockController : Controller
    {
        private readonly ISmartGoService _smartGoService;
        private readonly ISmartgoImportationService _smartgoImportationService;
        private readonly IReceiptNoteRepository _receiptNoteRepository;

        public ImportSmartGoStockController(ISmartGoService smartGoService, ISmartgoImportationService smartgoImportationService, IReceiptNoteRepository receiptNoteRepository)
        {
            _smartGoService = smartGoService;
            _smartgoImportationService = smartgoImportationService;
            _receiptNoteRepository = receiptNoteRepository;
        }

        public IActionResult Index()
        {
            StockImportViewModel stockViewModel = new StockImportViewModel()
            {
                ClientId = 0,
                ClientName = "",
                DepositanteId = 0
            };
            return View(stockViewModel);
        }

        public async Task<IActionResult> GetStockById(int id, int clientId, string clientName)
        {
            try
            {
                if (id == 0)
                    TempData["ErrorMessage"] = "Não foi encontrada nenhuma informação com ID indicado";

                List<SaldoDetalhado> list = await _smartGoService.GetSaldoDetalhadoByDepositorId(id);
                //List<EstoqueSimplificado> list = await _smartGoService.GetSimplifiedStockByDepositorId(id);
                
                if(list.Count <= 0)
                {
                    TempData["ErrorMessage"] = "Não foi encontrada nenhuma informação com ID indicado";
                    StockImportViewModel stockViewModelEmpty = new StockImportViewModel()
                    {
                        StockItems = new List<SmartgoImportation>(),
                        ClientId = 0,
                        ClientName = "",
                        DepositanteId = 0
                    };
                    return View("Index", stockViewModelEmpty);
                }

                var dbList = _smartgoImportationService.GenerateSmartgoImportation(id, list);

                StockImportViewModel stockViewModel = new StockImportViewModel()
                {
                    StockItems = dbList,
                    ClientId = clientId,
                    ClientName = clientName,
                    DepositanteId = id
                };

                return View("Index", stockViewModel);
            } catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Não foi possível realizar essa operação, por favor, tente novamente!";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public StatusMessageResponse CreateReciptNoteByImportStockItems(int idDepositante, int clientId, int[] itemIdsToImport)
        {
            try
            {
                if (clientId <= 0)
                    return new StatusMessageResponse { Message = "Não foi possível achar o cliente informado", Status = "error" };

                List<SmartgoImportation> list = _smartgoImportationService.GetItemsByIdList(idDepositante, itemIdsToImport);

                Random random = new Random();

                // Gerar 10 números aleatórios e convertê-los em strings
                var numerosAleatorios = Enumerable.Range(1, 10).Select(_ => random.Next(0, 10).ToString());

                // Concatenar os números aleatórios em uma única string
                string numerosConcatenados = string.Join("", numerosAleatorios);

                ReceiptNote receiptNote = new ReceiptNote()
                {
                    Number = clientId.ToString(),
                    SerialNumber = numerosConcatenados.ToString(),
                    AccessKey = numerosConcatenados,
                    EmitDocument = numerosConcatenados,
                    DestDocument = numerosConcatenados,
                    EntryDate = DateTime.Now,
                    IssueDate = DateTime.Now,
                    Status = NoteStatus.EmAndamento,
                    ClientId = clientId
                };

                List<ReceiptNoteItem> receiptNoteItemList = new List<ReceiptNoteItem>();

                foreach (var item in list)
                {
                    decimal price = 0m;
                    try
                    {
                        price = decimal.Parse(item.UnitPrice.UnmaskOnlyNumbers());
                        price = price / item.Quantity / 100.0m;
                    }
                    catch { }

                    ReceiptNoteItem noteItem = new ReceiptNoteItem()
                    {
                        Code = item.SKU,
                        Ean = item.EAN,
                        Quantity = item.Quantity,
                        QuantityInspection = item.Quantity,
                        Value = price,
                        Description = item.ProductName,
                        ReceiptNoteId = receiptNote.Id,
                        ItemStatus = NoteItemStatus.Finalizado,
                        PositionAddress = item.PositionAddress,
                        Validade = item.Validade = item.Validade,
                        Lote = item.Lote
                    };

                    receiptNoteItemList.Add(noteItem);
                }
                receiptNote.ReceiptNoteItems = receiptNoteItemList;

                _receiptNoteRepository.Add(receiptNote);

                return new StatusMessageResponse { Message = $"Nota de recebimento gerada com sucesso! Recebimento: {receiptNote.Id}", Status = "success" };

            }
            catch (Exception ex)
            {
                return new StatusMessageResponse { Message = "Não foi possível gerar a nota de recebimento, erro: " + ex, Status = "error" };

            }

        }
    }
}
