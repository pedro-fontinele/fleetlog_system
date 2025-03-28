using LOGHouseSystem.Adapters.BaseOAUTH2Extension.Dto;
using LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto;
using LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Request;
using LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Response;

namespace LOGHouseSystem.Adapters.Extensions.ShopeeExtension
{
    public interface IAPIShopeeService
    {
        Task GetAccessNewToken(string authorizationCode, int clientId);
        Task<GetShipmentListShiptShopeeResponseDto> GetShipmentList(int clientId, int pageSize = 9999, bool secondTrying = false);
        Task<HttpResponseMessage> GeneratingShippingDocumentAsync(CreateShippingDocumentShopeeRequestDto orderList, int clientId, bool secondTrying = false);
        Task<HttpResponseMessage> DownloadShippingDocumentAsync(List<ShipmentListShiptShopeeDto> orderList, string documentType, int clientId, bool secondTrying = false);
        Task<string> GenerateAuthorizationCodeUrl(int clientId);
        string GenerateAuthorizationCodeUrlSave(int clientId);
        Task<AuthenticationDto> GetAccessToken(int clientId, bool tryRefresh = false);
        Task<HttpResponseMessage> GetShippingDetails(string orderSn, int clientId, bool secondTrying = false);
        Task<HttpResponseMessage> ShipOrderAsync(ShipOrderShopeeRequest data, int clientId, bool secondTrying = false);
        Task<HttpResponseMessage> GetTrackingNumberAsync(string orderSn, int clientId, bool secondTrying = false);
        Task<HttpResponseMessage> GetShippingParamterAsync(string orderSn, int clientId, bool secondTrying = false);
        Task<HttpResponseMessage> GetShippingDocumentResultAsync(CreateShippingDocumentShopeeRequestDto orderList, int clientId, bool secondTrying = false);
    }
}
