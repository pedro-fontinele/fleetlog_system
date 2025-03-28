using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request;
using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Response;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using MailKit.Search;

namespace LOGHouseSystem.Services
{
    public class PickingListItemService : IPickingListItemService
    {
        private readonly IPickingListItemRepository _pickingListItemRepository;
        private readonly IPickingListRepository _pickingListRepository;
        private readonly IPackingService _packingService;
        private readonly IPickingListHistoryService _pickingListHistoryService;
        private readonly IExpeditionOrderService _expeditionOrderService;
        private readonly IExpeditionOrderRepository _expeditionOrderRespository;
        private readonly IExpeditionOrderHistoryService _expeditionOrderHistoryService;

        public PickingListItemService(IPickingListItemRepository pickingListItemRepository, 
            IPickingListRepository pickingListRepository,
            IPackingService packingService, 
            IExpeditionOrderService expeditionOrderService, 
            IExpeditionOrderRepository expeditionOrderRespository, 
            IPickingListHistoryService pickingListHistoryService, 
            IExpeditionOrderHistoryService expeditionOrderHistoryService)
        {
            _pickingListItemRepository = pickingListItemRepository;
            _pickingListRepository = pickingListRepository;
            _packingService = packingService;
            _expeditionOrderService = expeditionOrderService;
            _expeditionOrderRespository = expeditionOrderRespository;
            _pickingListHistoryService = pickingListHistoryService;
            _expeditionOrderHistoryService = expeditionOrderHistoryService;
        }

        public PickingListItem SearchItemToValidate(int PickingListId, string code)
        {
            return _pickingListItemRepository.GetAll()
                .Where(x => x.PickingListId == PickingListId && (x.Product.Ean == code || x.Product.Code == code))
                .FirstOrDefault();
        }

        public ValidatePickingListItemResponse Validate(ValidatePickingListItemRequest data, int userId)
        {
            var pickingListItem = SearchItemToValidate(data.PickingListId, data.Ean);

            if (pickingListItem is null) throw new Exception("Produto não encontrado");

            if (pickingListItem.QuantityInspection + 1 > pickingListItem.Quantity) throw new Exception("Produto já atingiu quantidade da lista de seleção");

            pickingListItem.QuantityInspection++;

            if (pickingListItem.QuantityInspection < pickingListItem.Quantity)
                pickingListItem.ItemStatus = PickingListItemStatus.EmAndamento;
                

            if (pickingListItem.QuantityInspection == pickingListItem.Quantity)
                pickingListItem.ItemStatus = PickingListItemStatus.Finalizado;
            
               

            pickingListItem = _pickingListItemRepository.Update(pickingListItem);


            var pickingList = _pickingListRepository.GetById(data.PickingListId);
            List<ExpeditionOrder> orders = pickingList.ExpeditionOrder;

            if (pickingList == null) throw new Exception("Item não encontrado");

            var allItemsRead = _pickingListItemRepository.GetByPickingListId(data.PickingListId).Where(x => x.PickingListId == data.PickingListId && x.ItemStatus != PickingListItemStatus.Finalizado).ToList();

            if (pickingList.Status == PickingListStatus.Gerado)
            {
                pickingList.Status = PickingListStatus.EmAtendimento;
            }

            if (!allItemsRead.Any())
            {
                pickingList.Status = PickingListStatus.Finalizado;
                _pickingListHistoryService.Add(data.PickingListId, "", PickingListStatus.Finalizado, userId);
                List<Packing> packings = _packingService.GeneratePackingByPickingId(pickingList.Id, userId);

            }


            pickingList = _pickingListRepository.Update(pickingList);

            return new ValidatePickingListItemResponse
            {
                PickingListItem = pickingListItem,
                PickingListStatus = pickingList.Status
            };
        }
    }
}
