using DocumentFormat.OpenXml.Drawing.Charts;
using LOGHouseSystem.Adapters.Extensions.NFeExtension;
using LOGHouseSystem.Controllers;
using LOGHouseSystem.Controllers.MVC;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using System.Text;
using static LOGHouseSystem.Services.ReceiptNoteLotsService;

namespace LOGHouseSystem.Services
{
    public class ReturnInvoiceService : IReturnInvoiceService
    {

        private readonly IReturnInvoiceRepository _returnInvoiceRepository;
        private readonly IReturnInvoiceItemRepository _returnInvoiceItemRepository;
        private readonly IReceiptNoteLotsService _receiptNoteLotsService;
        private readonly IProductRepository _productRepository;
        private readonly IReceiptNoteRepository _receiptNoteRepository;
        private readonly IReceiptNoteItemRepository _receiptNoteItemRepository;
        private readonly IReturnInvoiceOrdersRepository _returnInvoiceOrdersRepository;
        private readonly IExpeditionOrderRepository _expeditionOrderRepository;
        private readonly IFileService _fileService;
        private readonly INFeService _nFeService;
        private readonly IReturnInvoiceProductInvoicesRepository _returnInvoiceProductInvoicesRepository;        
        private readonly IExpeditionOrderService _expeditionOrderService;

        public ReturnInvoiceService(IReturnInvoiceRepository returnInvoiceRepository,
            IExpeditionOrderService expeditionOrderService,
            IReturnInvoiceItemRepository returnInvoiceItemRepository,
            IReceiptNoteLotsService receiptNoteLotsService,
            IProductRepository productRepository,
            IReceiptNoteRepository receiptNoteRepository,
            IReceiptNoteItemRepository receiptNoteItemRepository,
            IReturnInvoiceOrdersRepository returnInvoiceOrdersRepository,
            IExpeditionOrderRepository expeditionOrderRepository,
            IFileService fileService,
            INFeService nFeService,
            IReturnInvoiceProductInvoicesRepository returnInvoiceProductInvoicesRepository)
        {
            _returnInvoiceRepository = returnInvoiceRepository;
            _expeditionOrderService = expeditionOrderService;
            _returnInvoiceItemRepository = returnInvoiceItemRepository;
            _receiptNoteLotsService = receiptNoteLotsService;
            _productRepository = productRepository;
            _receiptNoteRepository = receiptNoteRepository;
            _receiptNoteItemRepository = receiptNoteItemRepository;
            _returnInvoiceOrdersRepository = returnInvoiceOrdersRepository;
            _expeditionOrderRepository = expeditionOrderRepository;
            _fileService = fileService;
            _nFeService = nFeService;
            _returnInvoiceProductInvoicesRepository = returnInvoiceProductInvoicesRepository;            
        }

        public Task<ReturnInvoice> AddAsync(ReturnInvoice invoice)
        {
            if (invoice == null) throw new ArgumentNullException("Não foi possível criar essa nota de devolução, por favor, tente novamente!");

            return _returnInvoiceRepository.Add(invoice);
        }

        public async Task<List<ReturnInvoice>> AddReturnInvoiceAndItems(List<ExpeditionOrder> expeditionOrders)
        {
            List<ExpeditionOrderItem> items = new List<ExpeditionOrderItem>();

            expeditionOrders.ForEach(e => items.AddRange(e.ExpeditionOrderItems));

            var productGroups = items.GroupBy(e => e.ProductId);

            await ValidadeIfAllProductIdsAreCreatedInDatabase(productGroups);

            List<OrdersLots> orders = await GetAllProductLots(productGroups);

            List<ReturnInvoice> returnInvoiceInserted = await CreateReturnInvoiceBasedInOrdersLots(orders, expeditionOrders);

            await VinculeReturnInvoicesWithExpeditionOrders(expeditionOrders, returnInvoiceInserted);

            return returnInvoiceInserted;
        }

        private async Task ValidadeIfAllProductIdsAreCreatedInDatabase(IEnumerable<IGrouping<int?, ExpeditionOrderItem>> productGroups)
        {
            string message = "";

            foreach (var productId in productGroups)
            {
                var product = await _productRepository.GetByIdAsync(productId.Key);

                if (product == null)
                {
                    var expedition = productId.First();

                    message = $"O produto {expedition.Ean} - {expedition.Description} - Não está cadastrado no armazem. Product Id {expedition.ProductId}";
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                throw new Exception(message);
            }
        }

        private async Task<List<OrdersLots>> GetAllProductLots(IEnumerable<IGrouping<int?, ExpeditionOrderItem>> productGroups){

            List<OrdersLots> orders = new List<OrdersLots>();

            foreach (var product in productGroups)
            {
                var productId = GetProductId(product);
                var sumQuantity = GetAllProductsQuantity(product);
                var expeditionOrderIds = GetExpeditionOrderIdsAbleToProcess(product);

                var notesListResponse = await _receiptNoteLotsService.ProcessLotAsync(productId, product.ToList(), Convert.ToDouble(sumQuantity));

                orders = ConvertNoteResponseInOrdersLot(productId, sumQuantity, expeditionOrderIds, notesListResponse, orders);
            }

            return orders;
        }
        private List<int> GetExpeditionOrderIdsAbleToProcess(IGrouping<int?, ExpeditionOrderItem> product)
        {
            return product.Select(e => Convert.ToInt32(e.ExpeditionOrderId)).Distinct().ToList();
        }

        private decimal GetAllProductsQuantity(IGrouping<int?, ExpeditionOrderItem> product)
        {
            return product.Sum(e => e.Quantity);
        }

        private int GetProductId(IGrouping<int?, ExpeditionOrderItem> product)
        {
            return Convert.ToInt32(product.Key);
        }

        private List<OrdersLots> ConvertNoteResponseInOrdersLot(int productId, decimal sumQuantity, List<int> expeditionOrderIds, NoteListResponse notesListResponse, List<OrdersLots> orders)
        {
            var notesListCoun = notesListResponse.NoteList.Count();

            for (int i = 0; i < notesListCoun; i++)
            {
                orders = CreateOrUpdateOrdersLots(i, productId, sumQuantity, expeditionOrderIds, notesListResponse, orders);
            }

            return orders;
        }

        private List<OrdersLots> CreateOrUpdateOrdersLots(int indice, int productId, decimal sumQuantity, List<int> expeditionOrderIds, NoteListResponse notesListResponse, List<OrdersLots> orders)
        {
            var productLots = new ProductNotes()
            {
                ProductId = productId,
                Quantity = sumQuantity,
                ExpeditionOrderIds = expeditionOrderIds,
                Notes = notesListResponse.NoteList[indice],                
            };

            if (orders.Count() < indice + 1)
            {
                orders.Add(new OrdersLots());
            }

            orders[indice].Products.Add(productLots);

            return orders;
        }



        private async Task<List<ReturnInvoice>> CreateReturnInvoiceBasedInOrdersLots(List<OrdersLots> orders, List<ExpeditionOrder> expeditionOrders)
        {
            List<ReturnInvoice> returnInvoiceInserted = new List<ReturnInvoice>();

            foreach (var order in orders)
            {
                ReturnInvoice returnInvoice = ConvertOrderLotToReturnInvoice(expeditionOrders);

                returnInvoice = await AddAsync(returnInvoice);                

                returnInvoice.ReturnInvoiceItems = await CreateItemsFromOrderLotProducts(order.Products, returnInvoice);                

                await _returnInvoiceItemRepository.AddListAsync(returnInvoice.ReturnInvoiceItems);

                returnInvoiceInserted.Add(returnInvoice);
            }

            return returnInvoiceInserted;
        }

        private async Task<List<ReturnInvoiceItem>> CreateItemsFromOrderLotProducts(List<ProductNotes> products, ReturnInvoice returnInvoice)
        {

            List<ReturnInvoiceItem> itemsInvoice = new List<ReturnInvoiceItem>();

            foreach (var productInvoice in products)
            {
                var product = await _productRepository.GetByIdAsync(productInvoice.ProductId);

                ReceiptNoteItem receiptNoteItem = await _receiptNoteItemRepository.GetByReceiptNoteAndEanAsync(productInvoice.Notes.ReceiptNoteId, product.Ean);

                if (receiptNoteItem == null)
                {
                    receiptNoteItem = await _receiptNoteItemRepository.GetByReceiptNoteAndCodeAsync(productInvoice.Notes.ReceiptNoteId, product.Code);
                }

                ReturnInvoiceItem returnInvoiceItem = new ReturnInvoiceItem()
                {
                    Name = "",
                    Quantity = Convert.ToDecimal(productInvoice.Notes.Quantity),
                    Description = receiptNoteItem.Description,
                    ExternalNumberItem = receiptNoteItem.Code,
                    Value = receiptNoteItem.Value,
                    Ean = receiptNoteItem.Ean,
                    ProductId = productInvoice.ProductId,
                    ReturnInvoiceId = returnInvoice.Id,
                    ReceiptNoteItemId = productInvoice.Notes.ReceiptNoteId
                };

                itemsInvoice.Add(returnInvoiceItem);
            }

            return itemsInvoice;
        }

        private ReturnInvoice ConvertOrderLotToReturnInvoice(List<ExpeditionOrder> expeditionOrders)
        {

            ReturnInvoice returnInvoice = new ReturnInvoice()
            {
                CreatedAt = DateTime.Now,
                Status = ReturnInvoiceStatus.Criada,
                //ExternalId = expeditionOrders.First().ExternalNumber != null ? expeditionOrders.First().ExternalNumber : "0",
                Value = _expeditionOrderService.GetTotalValueInListOfExpeditionOrders(expeditionOrders),
                ClientId = expeditionOrders.First().ClientId,
            };

            return returnInvoice;
        }

        public Task<PaginationBase<ReturnInvoice>> GetAllByPaginationBase(PaginationRequest request)
        {
            if (request.PageNumber <= 0)
                request.PageNumber = 1;

            return _returnInvoiceRepository.GetByPagination(request);
        }

        private async Task VinculeReturnInvoicesWithExpeditionOrders(List<ExpeditionOrder> expeditionOrders, List<ReturnInvoice> returnInvoiceInserted)
        {
            List<ProductAux> productAuxList = new List<ProductAux>();

            foreach (var order in expeditionOrders)
            {
                foreach (var item in order.ExpeditionOrderItems)
                {
                    var productsInserteds = GetAllProductsInsertetInReturnInvoiceList(returnInvoiceInserted, item.ProductId);

                    if (productsInserteds.Count() > 0)
                    {
                        var productAux = GetTemporaryProductProcessed(productAuxList, item.ProductId);

                        if (ProductIsProcessing(productAux))
                        {
                            await ContinueProcessingProduct(item, order, productAux, productsInserteds);
                        }
                        else
                        {
                            await AddNewProductAux(item, order, productAuxList, productsInserteds);
                        }
                    }
                }
            }
        }

        private async Task AddNewProductAux(ExpeditionOrderItem item, ExpeditionOrder order, List<ProductAux> productAuxList, IEnumerable<ReturnInvoiceItem> productsInserteds)
        {
            var returnInsert = productsInserteds.FirstOrDefault();

            productAuxList.Add(new ProductAux()
            {
                ProductId = Convert.ToInt32(returnInsert.ProductId),
                QuantityAux = item.Quantity
            });

            await AddReturnInvoiceOrder(order, returnInsert.ReturnInvoiceId, returnInsert.ProductId, item.Quantity);
        }

        private async Task ContinueProcessingProduct(ExpeditionOrderItem item, ExpeditionOrder order, ProductAux productAux, IEnumerable<ReturnInvoiceItem> productsInserteds)
        {
            decimal? previouslyCalculatedQuantity = 0;

            foreach (var itemInserted in productsInserteds)
            {
                if (WasThisReturnInvoiceQuantityCalculatedPreviously(previouslyCalculatedQuantity, itemInserted, productAux))
                {
                    previouslyCalculatedQuantity += itemInserted.Quantity;
                    continue;
                }

                decimal? remainingQuantity = GetRemainingQuantityInReturnInvoiceQuantity(itemInserted, productAux, previouslyCalculatedQuantity);

                await VinculeOrderIfThereIsRemainingQuantity(remainingQuantity, order, itemInserted);                

                if (!IsTherePendingOrderQuantityToProcess(item, remainingQuantity))
                {
                    productAux.QuantityAux += item.Quantity;
                    break;
                }
                else
                {
                    productAux.QuantityAux += Convert.ToDecimal(remainingQuantity);
                    previouslyCalculatedQuantity += itemInserted.Quantity;
                }
            }
        }

        private bool IsTherePendingOrderQuantityToProcess(ExpeditionOrderItem item, decimal? remainingQuantity)
        {
            if(item.Quantity <= remainingQuantity)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool WasThisReturnInvoiceQuantityCalculatedPreviously(decimal? previouslyCalculatedQuantity, ReturnInvoiceItem itemInserted, ProductAux productAux)
        {
            if (previouslyCalculatedQuantity + itemInserted.Quantity <= productAux.QuantityAux)
            {                
                return true;
            }

            return false;
        }

        private decimal? GetRemainingQuantityInReturnInvoiceQuantity(ReturnInvoiceItem itemInserted, ProductAux productAux, decimal? previouslyCalculatedQuantity)
        {
            decimal? remainingQuantity = itemInserted.Quantity;

            if (productAux.QuantityAux <= (previouslyCalculatedQuantity + itemInserted.Quantity))
            {
                remainingQuantity = (previouslyCalculatedQuantity + itemInserted.Quantity) - productAux.QuantityAux;
            }

            return remainingQuantity;
        }

        private async Task VinculeOrderIfThereIsRemainingQuantity(decimal? remainingQuantity, ExpeditionOrder order, ReturnInvoiceItem itemInserted)
        {
            if (remainingQuantity >= 0)
            {
                await AddReturnInvoiceOrder(order, itemInserted.ReturnInvoiceId, itemInserted.ProductId, itemInserted.Quantity);
            }
        }

        private async Task AddReturnInvoiceOrder(ExpeditionOrder order, int? returnInvoiceId, int? productId, decimal? quantity)
        {
            var returnInvoiceOrder = new ReturnInvoiceOrders()
            {
                ExpeditionOrderId = order.Id,
                ReturnInvoiceId = Convert.ToInt32(returnInvoiceId),
                ProductId = Convert.ToInt32(productId),
                Quantity = Convert.ToDecimal(quantity)
            };

            await _returnInvoiceOrdersRepository.AddAsync(returnInvoiceOrder);

            order.ReturnedInvoiceGenerated = YesOrNo.Yes;

            await _expeditionOrderRepository.UpdateAsync(order);
        }

        private IEnumerable<ReturnInvoiceItem> GetAllProductsInsertetInReturnInvoiceList(List<ReturnInvoice> returnInvoiceInserted, int? productId)
        {
            return returnInvoiceInserted.Where(e => e.ReturnInvoiceItems.Any(e => e.ProductId == productId)).Select(e => e.ReturnInvoiceItems.Where(f => f.ProductId == productId).FirstOrDefault()).ToList();
        }

        private ProductAux GetTemporaryProductProcessed(List<ProductAux> productAuxList, int? producId)
        {
            return productAuxList.Where(e => e.ProductId == producId).FirstOrDefault();
        }
        private bool ProductIsProcessing(ProductAux productAux)
        {
            return productAux != null;
        }

        public async Task<ReturnInvoiceCompleteViewModel> GetReturnInvoiceCompleteData(int returnInvoiceId)
        {
            var returnInvoiceCompleteViewModel = new ReturnInvoiceCompleteViewModel();
            returnInvoiceCompleteViewModel.Invoice = await _returnInvoiceRepository.GetByIdAsync(returnInvoiceId);

            List<ReturnInvoiceOrders> returnInvoiceOrders = await _returnInvoiceOrdersRepository.GetByReturnInvoiceId(returnInvoiceId);

            returnInvoiceCompleteViewModel.Orders = returnInvoiceOrders.Select(e => new ReturnInvoiceOrdersDetails()
            {
                Description = e.Product.Description,
                Ean = e.Product.Ean,
                Quantity = e.Quantity,
                ProductId = e.ProductId,
                InvoiceNumber = e.ExpeditionOrder.InvoiceNumber,
                ExpeditionId = e.ExpeditionOrder.Id,
            }).ToList();

            List<ReturnInvoiceProductInvoices> returnInvoiceProductInvoices = await _returnInvoiceProductInvoicesRepository.GetByReturnInvoiceId(returnInvoiceId);

            returnInvoiceCompleteViewModel.ReturnInvoiceProductInvoices = returnInvoiceProductInvoices;

            return returnInvoiceCompleteViewModel;

        }

        public async Task<List<ReturnInvoiceOrdersResponseViewModel>> GetReturnInvoicesFromOrder(int expeditionOrderId)
        {
            List<ReturnInvoiceOrders> returnInvoiceOrders = await _returnInvoiceOrdersRepository.GetByExpeditionOrderId(expeditionOrderId);

            var returnInvoiceOrdersResponseViewModel = returnInvoiceOrders.Select(e => new ReturnInvoiceOrdersResponseViewModel()
            {
                ReturnInvoiceId = e.ReturnInvoiceId,
                Product = e.Product.Description,
                InvoiceAccessKey = e.ReturnInvoice.InvoiceAccessKey ?? "",
                Quantity = e.Quantity
            }).ToList();

            return returnInvoiceOrdersResponseViewModel;
        }

        public async Task AddXmlFile(List<IFormFile> file, int returnInvoiceId)
        {
            var notes = new List<ReturnInvoiceAuxNotes>();

            foreach (var item in file)
            {
                _fileService.IsXmlFile(item);

                var xml = _fileService.ReadFile(item);

                var generatedNfe = _nFeService.GenerateNfeProc(xml, 0, 0, false, false);

                if (!generatedNfe.Success)
                {
                    throw new Exception(generatedNfe.Message);
                }

                var invoice = await _returnInvoiceProductInvoicesRepository.GetByAccessKey(generatedNfe.NFeProc.ProtNFe.InfProt.ChNFe, returnInvoiceId);

                if (invoice.Count() > 0)
                {
                    continue;
                }

                notes.Add(new ReturnInvoiceAuxNotes()
                {
                    Note = generatedNfe.NFeProc,
                    Xml = xml
                });
            }

            foreach (var note in notes)
            {
                var path = await _fileService.SaveXMLFile(note.Note.ProtNFe.InfProt.ChNFe, note.Xml);                

                var returnInvoice = new ReturnInvoiceProductInvoices()
                {
                    ReturnInvoiceId = returnInvoiceId,
                    XmlPath = path,
                    InvoiceAccessKey = note.Note.ProtNFe.InfProt.ChNFe
                };

                await _returnInvoiceProductInvoicesRepository.Add(returnInvoice);
            }
        }

        public async Task DeleteAttachedXml(int id)
        {
            var file = await _returnInvoiceProductInvoicesRepository.GetByIdAsync(id);

            if (file != null) {

                _fileService.DeleteXMLFile(file.InvoiceAccessKey);

                await _returnInvoiceProductInvoicesRepository.Delete(file);
            }

            
        }
    }

    public class ReturnInvoiceAuxNotes
    {
        public string Xml { get; set; }
        public NfeProc Note { get; set; }
    }

    public class OrdersLots
    {
        public List<ProductNotes> Products { get; set; } = new List<ProductNotes>();
    }

    public class ProductAux
    {
        public decimal QuantityAux { get; set; }
        public int ProductId { get; set; }
    }

    public class ProductNotes
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public List<int> ExpeditionOrderIds { get; set; }
        public NoteList Notes { get; set; }
        public List<ExpeditionOrdersLotNotFounded> NotFounded { get; internal set; }
    }
}
