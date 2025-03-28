using DocumentFormat.OpenXml.Presentation;
using LOGHouseSystem.Controllers.API.PipedriveHook.Requests;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.Services.PositionAndProduct;
using LOGHouseSystem.ViewModels;
using WebSocketSharp;

namespace LOGHouseSystem.Services
{
    public class ProductService
    {
        private AppDbContext _db = new AppDbContext();
        private ProductRepository _productRepository;
        private ReceiptNoteRepository _receiptNoteRepository;
        private ClientsRepository _clientRepository;

        private readonly IReceiptNoteLotsService _receiptNoteLotsService;
        private readonly IInventoryMovementRepository _inventoryMovementRepository;
        private readonly IReceiptNoteItemRepository _receiptNoteItemRepository;
        private readonly IPositionAndProductService _positionsAndProductService;
        

        public ProductService(IInventoryMovementRepository inventoryMovementRepository = null, 
            IReceiptNoteItemRepository receiptNoteItemRepository = null, 
            IReceiptNoteLotsService receiptNoteLotsService = null,
            IPositionAndProductService positionsAndProductService = null)
        {
            _clientRepository = new ClientsRepository();
            _productRepository = new ProductRepository(_clientRepository);
            _receiptNoteRepository = new ReceiptNoteRepository();
            _inventoryMovementRepository = inventoryMovementRepository ?? new InventoryMovementRepository();
            _receiptNoteItemRepository = receiptNoteItemRepository ?? new ReceiptNoteItemRepository();
            _receiptNoteLotsService = receiptNoteLotsService ?? new ReceiptNoteLotsService();
            _positionsAndProductService = positionsAndProductService;
        }

        public bool AddStockItemToProduct(int receiptNoteId)
        {
            try
            {
                var receiptNote = _receiptNoteRepository.GetById(receiptNoteId);

                if (receiptNote == null) return false;

                foreach (var receiptNoteItem in receiptNote.ReceiptNoteItems)
                {

                    if(receiptNoteItem.Ean.Contains("SEM GTIN"))
                    {
                        receiptNoteItem.Ean = receiptNoteItem.Code;
                    }

                    var product = _productRepository.GetByEan(receiptNoteItem.Ean, receiptNote.ClientId);

                    if (product == null)
                    {
                        Product newProduct = new Product()
                        {
                            ClientId = receiptNote.ClientId,
                            Code = receiptNoteItem.Code,
                            Ean = receiptNoteItem.Ean,
                            Description = receiptNoteItem.Description,
                            StockQuantity = 0,
                            CreatedAt = DateTime.Now
                        };

                        newProduct = _productRepository.Add(newProduct);

                        //check if has pre addressing to associate
                        //this address come from smartgo stock integration
                        if (!string.IsNullOrEmpty(receiptNoteItem.PositionAddress))
                        {
                            try
                            {
                                _positionsAndProductService.AssociateProductToPosition(newProduct.Id, receiptNoteItem.PositionAddress);
                            }
                            catch { }
                        }                       
                        

                        receiptNoteItem.ProductId = newProduct.Id;

                        //CreateLot                        
                        product = AddStockToProduct(newProduct, receiptNoteItem.QuantityInspection, OriginInventoryMovimentEnum.ReceiptNote, receiptNote.EntryDate, $"Cadastro do produto {receiptNoteItem.ProductId} - {receiptNoteItem.Description} ref a nota {receiptNote.Number} - {receiptNote.AccessKey}");

                        receiptNoteItem.ProductId = newProduct.Id;

                        //CreateLot
                        _receiptNoteLotsService.CreateNewLot(newProduct.Id, receiptNoteItem, receiptNoteItem.QuantityInspection);
                    }
                    else
                    {
                        product = AddStockToProduct(product, receiptNoteItem.QuantityInspection, OriginInventoryMovimentEnum.ReceiptNote, receiptNote.EntryDate, $"Entrada do produto {receiptNoteItem.ProductId} - {receiptNoteItem.Description} ref a nota {receiptNote.Number} - {receiptNote.AccessKey}");

                        receiptNoteItem.ProductId = product.Id;

                        _receiptNoteLotsService.CreateNewLot(product.Id, receiptNoteItem, receiptNoteItem.QuantityInspection);
                    }

                    receiptNoteItem.ItemStatus = NoteItemStatus.AguardandoEnderecamento;
                    _receiptNoteItemRepository.Update(receiptNoteItem);

                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Houve um erro no momento da aprovação. {ex.Message} - {ex.InnerException} - {ex.StackTrace}");
                return false;
            }
        }

        public Product AddStockToProduct(Product product, double quantity, OriginInventoryMovimentEnum origin, DateTime? date , string? note = "")
        {
            product.StockQuantity += quantity;
            product = _productRepository.Update(product);            

            _inventoryMovementRepository.MappingEntryMovement(InventoryType.Entrada, quantity, product.StockQuantity, product.Id, StockSlotMovimentEnum.StockQuantity, origin, date, note);

            return product;
        }

        public Product RemoveStockProduct(Product product, double quantity, OriginInventoryMovimentEnum origin, DateTime? date, string? note = "")
        {
            product.StockQuantity -= quantity;
            product = _productRepository.Update(product);

            _inventoryMovementRepository.MappingEntryMovement(InventoryType.Saida, quantity, product.StockQuantity, product.Id, StockSlotMovimentEnum.StockQuantity, origin, date, note);

            return product;
        }

        public Product AddReservedStockFromStockProduct(Product product, double quantity, OriginInventoryMovimentEnum origin, DateTime? date, string? note = "")
        {
            RemoveStockProduct(product, quantity, origin, date, note);

            product.StockReservationQuantity += quantity;
            product = _productRepository.Update(product);

            _inventoryMovementRepository.MappingEntryMovement(InventoryType.Entrada, quantity, product.StockReservationQuantity, product.Id, StockSlotMovimentEnum.StockReservationQuantity, origin, date, note);

            return product;
        }

        public Product AddReservedStockToStockProduct(Product product, double quantity, OriginInventoryMovimentEnum origin, DateTime? date, string? note = "")
        {           

            product.StockReservationQuantity -= quantity;
            product = _productRepository.Update(product);

            _inventoryMovementRepository.MappingEntryMovement(InventoryType.Saida, quantity, product.StockReservationQuantity, product.Id, StockSlotMovimentEnum.StockReservationQuantity, origin, date, note);

            AddStockToProduct(product, quantity, origin, date, note);

            return product;
        }

        public Product RemoveReservedStockProduct(Product product, double quantity, OriginInventoryMovimentEnum origin, DateTime? date, string? note = "")
        {
            product.StockReservationQuantity -= quantity;
            product = _productRepository.Update(product);

            _inventoryMovementRepository.MappingEntryMovement(InventoryType.Saida, quantity, product.StockReservationQuantity, product.Id, StockSlotMovimentEnum.StockReservationQuantity, origin, date, note);

            return product;
        }

        //Confere se um produto existe, se não existir, cria um
        public Product CheckAndAddProduct(string ean, Product product)
        {
            Product productByEan = _productRepository.GetByEan(ean, product.ClientId);

            if(productByEan == null && !string.IsNullOrEmpty(ean))
            {
                Product productReturned = _productRepository.Add(product);
                return productReturned;
            }

            return productByEan;
        }



    }
}
