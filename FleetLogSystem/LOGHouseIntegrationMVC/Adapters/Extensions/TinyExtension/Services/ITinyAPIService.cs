using LOGHouseSystem.Adapters.Extensions.TinyExtension.Dto;
using LOGHouseSystem.Models;
using LOGHouseSystem.Services.Tiny.Dto;

namespace LOGHouseSystem.Adapters.Extensions.TinyExtension.Service
{
    public interface ITinyAPIService
    {
        Task<TinyCompleteOrderRequestDto> GetOrder(string id, Client client);
        Task<TinyCompleteProductDto> GetProduct(long id, Client client);
        Task<string?> GetXmlInvoice(int idTinyInvoice, Client client);
        Task<string> enviarRESTPOSTAsync(string url, string data, string optional_headers = null);
        Task<TinyInvoiceResponse> GetInvoice(string id, Client client);
    }
}
