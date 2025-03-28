using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request;
using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Response;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IPickingListItemService
    {
        PickingListItem SearchItemToValidate(int PickingListId, string code);

        ValidatePickingListItemResponse Validate(ValidatePickingListItemRequest data, int userId);
    }
}
