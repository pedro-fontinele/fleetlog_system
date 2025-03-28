using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request;
using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Response;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IPackingItemService
    {
        List<PackingItem> GetAllByPackingId(int id);

        List<PackingItem> MapPakingItemsByExpeditionOrderItems(List<ExpeditionOrderItem> orderItems);

        Task<ValidatePackingItemResponse> Validate(ValidatePackingItemRequest data, int userId);
    }
}
