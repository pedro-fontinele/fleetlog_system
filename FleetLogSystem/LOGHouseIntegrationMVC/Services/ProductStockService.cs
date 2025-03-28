using DocumentFormat.OpenXml.Drawing.Charts;
using LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace LOGHouseSystem.Services
{
    public class ProductStockService : IProductStockService
    {
        private IProductRepository _productRepository;
        private IInventoryMovementRepository _inventoryMovementRepository;
        private IExpeditionOrderRepository _expeditionOrderRepository;
        private IExpeditionOrderItemsRepository _expeditionOrderItemsRepository;
        private IReceiptNoteItemRepository _receiptNoteItemRepository;
        private ProductService _productService;
        private List<ExpeditionOrderStatus> expeditionOrdersStatusUsable;
        private List<NoteStatus> receiptNoteStatusNotUsable;

        public ProductStockService(
            IProductRepository productRepository,
            IInventoryMovementRepository inventoryMovementRepository,
            IExpeditionOrderItemsRepository expeditionOrderItemsRepository,
            IExpeditionOrderRepository expeditionOrderRepository,
            IReceiptNoteItemRepository receiptNoteItemRepository)
        {
            _productRepository = productRepository;
            _inventoryMovementRepository = inventoryMovementRepository;
            _expeditionOrderRepository = expeditionOrderRepository;
            _expeditionOrderItemsRepository = expeditionOrderItemsRepository;
            _receiptNoteItemRepository = receiptNoteItemRepository;
            _productService = new ProductService();

            receiptNoteStatusNotUsable = new List<NoteStatus>()
            {
                NoteStatus.AguardandoEnderecamento,
                NoteStatus.Finalizada                
            };

            expeditionOrdersStatusUsable = new List<ExpeditionOrderStatus>()
            {
                ExpeditionOrderStatus.Processed,
                ExpeditionOrderStatus.InPickingList,
                ExpeditionOrderStatus.Separated,
                ExpeditionOrderStatus.InPacking,
                ExpeditionOrderStatus.Packed,
                ExpeditionOrderStatus.Dispatched
            };
        }

        public async Task ProcessByProductId(int productId, DateTime dateTime)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                throw new Exception("Não foi possivel encontrar o produto");
            }

            await ProcessByProduct(product, dateTime);
        }

        public async Task ProcessByProduct(Product product, DateTime dateTime)
        {
            var baseDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);

            await DisableAllInventoryMoviments(product.Id, baseDate);

            List<ReceiptNoteItem> receiptNotes = await _receiptNoteItemRepository.GetAllByStatusAndMinimusDate(receiptNoteStatusNotUsable, baseDate, product.Code, product.Id, product.ClientId);

            List<ExpeditionOrderItem> expeditionOrders = await _expeditionOrderItemsRepository.GetAllByStatusOrdersIsNotAndMinimusDateAsync(expeditionOrdersStatusUsable, baseDate, product.Id, product.ClientId);

            _productService.RemoveStockProduct(product, product.StockQuantity, OriginInventoryMovimentEnum.Reprocessing, baseDate, $"O estoque do produto {product.Description} está sendo reprocessando - Limpando estoque principal");

            _productService.RemoveReservedStockProduct(product, product.StockReservationQuantity, OriginInventoryMovimentEnum.Reprocessing, baseDate, $"O estoque do produto {product.Description} está sendo reprocessando - Limpando estoque reservado");


            List<ReprocessData> data = new List<ReprocessData>();

            data.AddRange(receiptNotes.Select(e => new ReprocessData()
            {
                Date = e.ReceiptNote.EntryDate,
                Quantity = e.QuantityInspection,
                Type = TypeReprocess.ReceiptNote,
                Description = $"Entrada de estoque feito com reprocessamento baseado na nota {e.ReceiptNote.Number}"

            }).ToList());


            data.AddRange(expeditionOrders.Select(e => new ReprocessData()
            {
                Date = e.ExpeditionOrder.IssueDate,
                Quantity = Convert.ToDouble(e.Quantity),
                Type = TypeReprocess.ExpeditionOrder,
                Description = $"Movendo estoque para reserva feito com reprocessamento baseado na nota {e.ExpeditionOrder.InvoiceNumber}"

            }).ToList());

            var newExpeditionOrders = expeditionOrders.Where(e => e.ExpeditionOrder.Status == ExpeditionOrderStatus.Packed || e.ExpeditionOrder.Status == ExpeditionOrderStatus.Dispatched).ToList();

            data.AddRange(newExpeditionOrders.Select(e => new ReprocessData()
            {
                Date = e.ExpeditionOrder.IssueDate ?? e.ExpeditionOrder.CreationDate,
                Quantity = Convert.ToDouble(e.Quantity),
                Type = TypeReprocess.SendOrder,
                Description = $"Removento estoque da reserva feito com reprocessamento baseado na nota {e.ExpeditionOrder.InvoiceNumber}"

            }).ToList());

            data = data.OrderBy(e => e.Date).ToList();

            foreach (var item in data)
            {

                var productToUpdate = await _productRepository.GetByIdAsync(product.Id);

                if (item.Type == TypeReprocess.ReceiptNote)
                {
                    _productService.AddStockToProduct(productToUpdate, Convert.ToDouble(item.Quantity), OriginInventoryMovimentEnum.ReceiptNote, item.Date, item.Description);
                }
                else if (item.Type == TypeReprocess.ExpeditionOrder)
                {
                    _productService.AddReservedStockFromStockProduct(productToUpdate, Convert.ToDouble(item.Quantity), OriginInventoryMovimentEnum.ExpeditionOrder, item.Date, item.Description);
                }
                else if (item.Type == TypeReprocess.SendOrder)
                {
                    _productService.RemoveReservedStockProduct(productToUpdate, Convert.ToDouble(item.Quantity), OriginInventoryMovimentEnum.ExpeditionOrder, item.Date, item.Description);
                }
            }

            /*foreach (var note in receiptNotes)
            {
                _productService.AddStockToProduct(product, note.Quantity, OriginInventoryMovimentEnum.ReceiptNote, note.ReceiptNote.EntryDate, $"Entrada de estoque feito com reprocessamento baseado na nota {note.ReceiptNote.Number}");
            }

            foreach (var order in expeditionOrders)
            {
                _productService.AddReservedStockFromStockProduct(product, Convert.ToDouble(order.Quantity), OriginInventoryMovimentEnum.ExpeditionOrder, order.ExpeditionOrder.IssueDate, $"Movendo estoque para reserva feito com reprocessamento baseado na nota {order.ExpeditionOrder.InvoiceNumber}");
            }

            var newExpeditionOrders = expeditionOrders.Where(e => e.ExpeditionOrder.Status == ExpeditionOrderStatus.Packed || e.ExpeditionOrder.Status == ExpeditionOrderStatus.Dispatched).ToList();

            foreach (var order in newExpeditionOrders)
            {
                _productService.RemoveReservedStockProduct(product, Convert.ToDouble(order.Quantity), OriginInventoryMovimentEnum.ExpeditionOrder, order.ExpeditionOrder.IssueDate, $"Removento estoque da reserva feito com reprocessamento baseado na nota {order.ExpeditionOrder.InvoiceNumber}");
            }*/
        }

        public async Task ProcessByClient(int clientId, DateTime dateTime)
        {
            var products = await _productRepository.GetByClientIdAsync(clientId);

            foreach (var product in products)
            {
                await ProcessByProduct(product, dateTime);
            }
        }

        private async Task DisableAllInventoryMoviments(int productId, DateTime dateTime)
        {
            //await _inventoryMovementRepository.DeleteAllbyProductIdAndDate(productId, dateTime);
            await _inventoryMovementRepository.DisableAllByProductAndDate(productId, dateTime);
        }

        public async Task ProcessByClientId(int clientId, DateTime dateTime)
        {
            var products = await _productRepository.GetByClientIdAsync(clientId);

            foreach (var product in products)
            {
                await ProcessByProduct(product, dateTime);
            }
        }

        public void CreateHangFireProcessByClientId(int clientId, DateTime dateTime)
        {
            Hangfire.BackgroundJob.Schedule(
                            () => ProcessByClientId(clientId, dateTime), TimeSpan.FromSeconds(2));
        }

        public class ReprocessData
        {
            public double? Quantity { get; set; }
            public string Description { get; set; }
            public DateTime? Date { get; set; }
            public TypeReprocess Type { get; set; }
        }

        public enum TypeReprocess
        {
            ExpeditionOrder,
            ReceiptNote,
            SendOrder
        }
    }
}
