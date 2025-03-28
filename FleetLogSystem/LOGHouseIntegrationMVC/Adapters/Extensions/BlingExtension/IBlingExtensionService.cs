using LOGHouseSystem.Adapters.Extensions.BlingExtension.Dto.Hook.Request;
using RestSharp;

namespace LOGHouseSystem.Adapters.Extensions.BlingExtension
{
    public interface IBlingExtensionService
    {
        Task SetClientData(int clientId);
        Task<RestResponse> GetXmlByAccessKey(string chaveAcesso);
        Task<RestResponse> GetInvoiceData(string number, string serial);
        Task<RestResponse> GetXmlByXml(string xml);
        Task<RestResponse> GetOrder(string externalNumber);
        Task<BlingSituacaoPedidoCallbackRequest> GetOrder(string externalNumber, int clientId);
        Task<RestResponse> GetOrders(DateTime startDate, DateTime endDate, int clientId, int page = 1);
    }
}
