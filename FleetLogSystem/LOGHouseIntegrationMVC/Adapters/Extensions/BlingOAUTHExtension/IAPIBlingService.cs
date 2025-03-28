using LOGHouseSystem.Adapters.BaseOAUTH2Extension.Dto;
using LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto;
using LOGHouseSystem.Models;
using RestSharp;

namespace LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension
{
    public interface IAPIBlingService
    {
        Task GetAccessNewToken(string authorizationCode, int clientId);
        Task CreateV3IntegrationVariables(int integrationId, int clientId);
        Task<RestResponse?> SendNfe(BlingNfeRequestDto nfe, int clientId, bool secondTrying = false);
        Task<RestResponse?> ConfirmNfe(long idNfe, int clientId, bool secondTrying = false);
        Task<RestResponse?> GetEnvironmentOperation(string description, int clientId, bool secondTrying = false);
        Task<RestResponse?> UpdateNfe(BlingNfeRequestDto nfe, long idNfe, int clientId, bool secondTrying = false);
        Task<AuthenticationDto> GetAccessToken(int clientId, bool tryRefresh = false);
        Task<RestResponse> GetOrders(DateTime startDate, DateTime endDate, int clientId, bool secondTrying = false);
        Task<RestResponse> GetOrder(string orderNumber, int clientId, bool secondTrying = false);
        Task<RestResponse> GetNotesBySearch(string blingId, int clientId, bool secondTrying = false);
        Task<RestResponse> GetNoteByInvoiceId(string invoiceId, int clientId, bool secondTrying = false);
        Task<RestResponse> GetNFe(string id, int clientId, bool secondTrying = false);
    }
}
