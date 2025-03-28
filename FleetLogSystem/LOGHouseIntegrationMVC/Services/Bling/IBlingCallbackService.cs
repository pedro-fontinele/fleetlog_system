using LOGHouseSystem.Adapters.Extensions.BlingExtension.Dto.Hook.Request;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services
{
    public interface IBlingCallbackService
    {
        Task ReceiveOrder(BlingPedidoCallbackRequest pedido, string urlXml, string? cnpj = null);
        Task ProcessBlingOrder(HookInput order);
        Task CreateIntegrationVariables(int id);
        Task ProcessBlingInvoice(HookInput item);
    }
}
