using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Services
{
    public class ReceiptNoteLotsService : IReceiptNoteLotsService
    {
        public IReceiptNoteLotsRepository _receiptNoteLotsRepository;
        public IReceiptNoteRepository _receiptNoteRepository;
        private IExpeditionOrdersLotNotFoundedRepository _expeditionOrdersLotNotFoundedRepository;
        private IProductRepository _productRepository;
        private IReceiptNoteItemRepository _receiptNoteItemRepository;

        public ReceiptNoteLotsService(IExpeditionOrdersLotNotFoundedRepository expeditionOrdersLotNotFoundedRepository = null, IProductRepository productRepository = null, IReceiptNoteItemRepository receiptNoteItemRepository = null, IReceiptNoteLotsRepository receiptNoteLotsRepository = null, IReceiptNoteRepository receiptNoteRepository = null)
        {
            _receiptNoteLotsRepository = receiptNoteLotsRepository;
            _receiptNoteRepository = receiptNoteRepository;
            _expeditionOrdersLotNotFoundedRepository = expeditionOrdersLotNotFoundedRepository;
            _productRepository = productRepository;
            _receiptNoteItemRepository = receiptNoteItemRepository;
        }

        public async Task CreateLots(ReceiptNoteItem productReceipt)
        {

            Product product = null;

            if (productReceipt.ProductId != null && productReceipt.ProductId > 0)
            {
                product = await _productRepository.GetByIdAsync(productReceipt.ProductId);
            }

            if (product == null)
            {
                product = await _productRepository.GetByCodeAsync(productReceipt.Code, productReceipt.ReceiptNote.ClientId);
            }

            if (product == null)
            {
                throw new Exception(string.Format("Não foi possivel encontrar o produto Ean {0} - Code {1} - Descrição {2} - Nota de retorno {3} - Cliente {4}", productReceipt.Ean, productReceipt.Code, productReceipt.Description, productReceipt.ReceiptNote.Number, productReceipt.ReceiptNote.Client.SocialReason));
            }

            CreateNewLot(product.Id, productReceipt, productReceipt.Quantity);

            await SetPendingProductsNotFoundedCreated(product.Id);
        }

        private async Task SetPendingProductsNotFoundedCreated(int productId)
        {
            List<ExpeditionOrdersLotNotFounded> products = await _expeditionOrdersLotNotFoundedRepository.GetAllByProductIdAndStatus(productId, ExpeditionOrdersLotNotFoundedStatusEnum.Created);

            foreach (var product in products)
            {
                product.Status = ExpeditionOrdersLotNotFoundedStatusEnum.Pendenting;

                await _expeditionOrdersLotNotFoundedRepository.UpdateAsync(product);
            }
        }

        public async Task<NoteListResponse> ProcessLotAsync(int idProduct, List<ExpeditionOrderItem> items, double quantity)
        {
            var response = new NoteListResponse();

            var listLots = new List<NoteList>();

            double nextQuantityToProcess = quantity;

            while (true)
            {
                if (nextQuantityToProcess <= 0)
                {
                    break;
                }

                ReceiptNoteLots lot = await GetAbleReceiptNoteLots(idProduct);

                if (lot != null)
                {
                    (double remainingQuantity, NoteList lots) = await ProcessFoundedLot(lot, nextQuantityToProcess);

                    listLots.Add(lots);

                    if (remainingQuantity <= 0)
                    {                     
                        break;
                    }

                    nextQuantityToProcess = remainingQuantity;
                }
                else
                {
                    response.NoteListNotFounded = await ProcessLotNotFoundToProduct(items, quantity, nextQuantityToProcess);
                    break;
                }                
            }


            response.NoteList = listLots;

            return response;
        }

        private async Task<(double, NoteList)> ProcessFoundedLot(ReceiptNoteLots lot, double nextQuantityToProcess)
        {
            double remainingQuantity = await ProcessLogQuantityAsync(lot, nextQuantityToProcess);

            double noteListQuantity = nextQuantityToProcess - remainingQuantity;

            var lots = new NoteList { ReceiptNoteId = lot.ReceiptNoteId, Quantity = noteListQuantity, ProductId = Convert.ToInt32(lot.ProductId) };

            if (remainingQuantity > 0)
            {
                await CloseLotAsync(lot);
            }

            return (remainingQuantity, lots);
        }

        private async Task<List<ExpeditionOrdersLotNotFounded>> ProcessLotNotFoundToProduct(List<ExpeditionOrderItem> items, double totalQuantity, double remainingQuantity)
        {
            double auxQuantity = 0;
            double quantityFounded = totalQuantity - remainingQuantity;
            bool needsContinueToCalculate = true;
            List<ExpeditionOrdersLotNotFounded> notFoundedList = new List<ExpeditionOrdersLotNotFounded>();

            foreach (var item in items)
            {
                (decimal quantityOfProductNotFounded, auxQuantity) = GetQuantityOfProductNotFounded(item, auxQuantity, quantityFounded, needsContinueToCalculate);

                (needsContinueToCalculate, ExpeditionOrdersLotNotFounded notFounded) = await ProcessQuantityOfProductNotFounded(item, quantityOfProductNotFounded);

                if (notFounded != null)
                {
                    notFoundedList.Add(notFounded);
                }                
            }

            return notFoundedList;
        }

        private async Task<(bool, ExpeditionOrdersLotNotFounded)> ProcessQuantityOfProductNotFounded(ExpeditionOrderItem item, decimal quantityOfProductNotFounded)
        {
            if (quantityOfProductNotFounded > 0)
            {                
                var expeditionOrderProductsNotFounded = new ExpeditionOrdersLotNotFounded()
                {
                    Quantity = quantityOfProductNotFounded,
                    ExpeditionOrderId = Convert.ToInt32(item.ExpeditionOrderId),
                    ProductId = Convert.ToInt32(item.ProductId),
                    Status = ExpeditionOrdersLotNotFoundedStatusEnum.Created
                };

                await _expeditionOrdersLotNotFoundedRepository.AddAsync(expeditionOrderProductsNotFounded);

                return (false, expeditionOrderProductsNotFounded);
            }

            return (true, null);
        }

        private (decimal quantityOfProductNotFounded, double auxQuantity) GetQuantityOfProductNotFounded(ExpeditionOrderItem item, double auxQuantity, double quantityFounded, bool needsContinueToCalculate)
        {
            if (needsContinueToCalculate)
            {
                if (ProductHasPendingQuantity(item.Quantity, auxQuantity, quantityFounded))
                {
                    var quantityOfProductNotFounded = GetProductPendingQuantity(item.Quantity, auxQuantity, quantityFounded);

                    return (quantityOfProductNotFounded, auxQuantity);

                }
                else
                {
                    auxQuantity += Convert.ToDouble(item.Quantity);

                    return (0, auxQuantity);
                }
            }
            else
            {
               return (item.Quantity, auxQuantity);
            }
        }

        private bool ProductHasPendingQuantity(decimal productQuantity, double quantityFoundedInProducts, double quantityFounded)
        {
            return Convert.ToDouble(productQuantity) + quantityFoundedInProducts > quantityFounded;
        }

        private decimal GetProductPendingQuantity(decimal productQuantity, double quantityFoundedInProducts, double quantityFounded)
        {
            return Convert.ToDecimal((Convert.ToDouble(productQuantity) + quantityFoundedInProducts) - quantityFounded);
        }

        public ReceiptNoteLots? GetReceiptNote(int idProduto)
        {
            return _receiptNoteLotsRepository.GetByProductIdAndStatus(idProduto, LotStatus.EmAndamento).FirstOrDefault();
        }

        public async Task<ReceiptNoteLots?> GetReceiptNoteProcessingAsync(int idProduto)
        {
            return (await _receiptNoteLotsRepository.GetByProductIdAndStatusAsync(idProduto, LotStatus.EmAndamento)).FirstOrDefault();
        }

        public async Task<ReceiptNoteLots?> GetReceiptNoteGeneratedAsync(int idProduto)
        {
            var ableLots = await _receiptNoteLotsRepository.GetByProductIdAndStatusAsync(idProduto, LotStatus.Gerado);

            return ableLots.FirstOrDefault();
        }

        public void CreateNewLot(int product, ReceiptNoteItem receiptNoteItem, double inputQuantity)
        {
            if (receiptNoteItem.LotGenerated == YesOrNo.Yes)
            {
                return;
            }

            var receiptNoteLots = new ReceiptNoteLots(product, receiptNoteItem.ReceiptNoteId, inputQuantity, null);

            _receiptNoteLotsRepository.Add(receiptNoteLots);

            receiptNoteItem.LotGenerated = YesOrNo.Yes;

            _receiptNoteItemRepository.Update(receiptNoteItem);
        }

        private async Task<ReceiptNoteLots> GetAbleReceiptNoteLots(int idProduct)
        {
            ReceiptNoteLots lot = await GetReceiptNoteProcessingAsync(idProduct);

            if (lot == null || lot.Diference() <= 0)
            {
                if (lot != null)
                {
                    await CloseLotAsync(lot);
                }

                lot = await GetReceiptNoteGeneratedAsync(idProduct);

                if (lot != null)
                {
                    await StartNewLotAsync(lot);
                }                
            }

            return lot;
        }

        private async Task<double> ProcessLogQuantityAsync(ReceiptNoteLots lot, double quantity)
        {
            double remainingQuantity = 0;

            if (lot.Diference() >= quantity)
            {
                lot.OutputQuantity = lot.CalculateOutput(quantity);
            }
            else
            {
                remainingQuantity = quantity - lot.Diference();
                lot.OutputQuantity = lot.CalculateOutput(lot.Diference());
                lot.UpdatedAt = DateTime.Now;
                lot.Status = LotStatus.Finalizado;
            }            
            
            await _receiptNoteLotsRepository.UpdateAsync(lot);

            return remainingQuantity;
        }

        private async Task<ReceiptNoteLots> StartNewLotAsync(ReceiptNoteLots lot)
        {            
            lot.Status = LotStatus.EmAndamento;
            lot.UpdatedAt = DateTime.Now;
            return await _receiptNoteLotsRepository.UpdateAsync(lot);
        }
        private async Task CloseLotAsync(ReceiptNoteLots lot)
        {
            lot.OutputQuantity = lot.InputQuantity;
            lot.Status = LotStatus.Finalizado;
            lot.UpdatedAt = DateTime.Now;
            await _receiptNoteLotsRepository.UpdateAsync(lot);
        }

        public async Task<PaginationBase<ReceiptNoteLots>> GetAllLots(ReceiptNoteIndexLotsPaginationRequest request)
        {
            PaginationBase<ReceiptNoteLots> data = await _receiptNoteLotsRepository.GetAllAsync(request);

            return data;
        }

        public async Task UpdateStatus(int id, LotStatus status)
        {
            var item = await _receiptNoteLotsRepository.GetLotByIdAsync(id);

            item.Status = status;
            item.UpdatedAt = DateTime.Now;

            await _receiptNoteLotsRepository.UpdateAsync(item);
        }

        public async Task ActiveStatus(int id)
        {
            var item = await _receiptNoteLotsRepository.GetLotByIdAsync(id);

            var active = await _receiptNoteLotsRepository.GetByProductIdAndStatusAsync(Convert.ToInt32(item.ProductId), LotStatus.EmAndamento);

            if (active.Count > 0)
            {
                throw new Exception("Não foi possível realizar a ativação, o produto em questão já possui um lot ativo");
            }

            await UpdateStatus(id, LotStatus.Gerado);
        }

        public class NoteListResponse
        {
            public List<NoteList> NoteList { get; set; }
            public List<ExpeditionOrdersLotNotFounded> NoteListNotFounded { get; set; }
        }
        public class NoteList {
            public int ReceiptNoteId { get; set; }
            public int ExpeditionOrderId { get; set; }
            public int ProductId { get; set; }
            public double Quantity { get; set; }
        }
    }
}
