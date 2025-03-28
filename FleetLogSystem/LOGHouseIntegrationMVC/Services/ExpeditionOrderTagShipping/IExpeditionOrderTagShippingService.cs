using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services
{
    public interface IExpeditionOrderTagShippingService
    {
        Task UpdateShippingOrderIntegraded(string accessKey, ExpeditionOrderTagShipping orderShipping = null);
    }
}
