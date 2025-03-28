using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request;
using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Response;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services
{
    public class PackingItemService : IPackingItemService
    {
        private readonly IPackingItemRepository _packingItemRepository;
        private readonly IPackingRepository _packingRepository;
        private readonly IPackingHistoryService _packingHistoryService;

        public PackingItemService(IPackingItemRepository packingItemRepository,
                                  IPackingRepository packingRepository,
                                  IPackingHistoryService packingHistoryService)
        {
            _packingItemRepository = packingItemRepository;
            _packingRepository = packingRepository;
            _packingHistoryService = packingHistoryService;
        }

        public List<PackingItem> GetAllByPackingId(int id)
        {
            return _packingItemRepository.GetAllByPackingId(id);
        }


        public List<PackingItem> MapPakingItemsByExpeditionOrderItems(List<ExpeditionOrderItem> orderItems)
        {            

            List<PackingItem> packingItems = new List<PackingItem>();

            foreach (var item in orderItems)
            {
                PackingItem items = new()
                {
                    Quantity = item.Quantity,
                    ValidatedQuantity = 0,
                    Status = Infra.Enums.PackingItemStatus.Gerado,                    
                    ProductId = item.ProductId
                };

                packingItems.Add(items);
            }

            return packingItems;
        }

        public PackingItem SearchItemToValidate(int PackingId, string code)
        {
            return _packingItemRepository.GetAll()
                .Where(x => x.PackingId == PackingId && (x.Product.Ean == code || x.Product.Code == code) && x.Status != PackingItemStatus.Finalizado)
                .FirstOrDefault();
        }

        public async Task<ValidatePackingItemResponse> Validate(ValidatePackingItemRequest data, int userId)
        {
            var packingItem = SearchItemToValidate(data.PackingId, data.Ean);

            if (packingItem is null) throw new Exception("Produto não encontrado");

            if (packingItem.ValidatedQuantity + 1 > packingItem.Quantity) throw new Exception("Produto já atingiu quantidade da lista de seleção");

            packingItem.ValidatedQuantity++;

            if (packingItem.ValidatedQuantity < packingItem.Quantity)
                packingItem.Status = PackingItemStatus.EmAndamento;

            if (packingItem.ValidatedQuantity == packingItem.Quantity)
                packingItem.Status = PackingItemStatus.Finalizado;

            packingItem = _packingItemRepository.Update(packingItem);

            var packing = _packingRepository.GetById(data.PackingId);

            if (packing == null) throw new Exception("Item não encontrado");

            var allItemsRead = _packingItemRepository.GetAllByPackingId(data.PackingId).Where(x => x.PackingId == data.PackingId && x.Status != PackingItemStatus.Finalizado).ToList();

            if (packing.Status == PackingStatus.Gerado)
            {
                packing.Status = PackingStatus.EmAtendimento;
            }                

            if (!allItemsRead.Any())
                packing.Status = PackingStatus.Finalizado;
                await _packingHistoryService.Add(packing.Id, "", PackingStatus.Finalizado, userId);

            packing = await _packingRepository.UpdateAsync(packing);

            return new ValidatePackingItemResponse
            {
                PackingItem = packingItem,
                PackingStatus = packing.Status
            };
        }
    }
}
