using LOGHouseSystem.Adapters.Extensions.TinyExtension.Dto.Hook.Request;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services
{
    public interface ITinyHookService
    {
        public Task ReceiveOnder(TinyInvoiceWebhookRequest order);
        Task CreateIntegrationVariables(int id);

        Task ProcessTinyOrder(HookInput order);
    }
}
