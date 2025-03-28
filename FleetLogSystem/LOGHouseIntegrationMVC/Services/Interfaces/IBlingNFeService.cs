using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IBlingNFeService
    {
        Task SendNfeFromReturnInvoice(int returnInvoiceId, int clientIdEmit);

        Task SendNfeFromReturnInvoice(ReturnInvoice returnInvoice, int clientIdEmit);
    }
}
