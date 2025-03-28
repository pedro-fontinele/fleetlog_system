using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;

namespace LOGHouseSystem.Services
{
    public class ExpeditionOrderTagShippingService : IExpeditionOrderTagShippingService
    {        
        private IExpeditionOrderTagShippingRepository _expeditionOrderShippingRepository;
        private IExpeditionOrderRepository _expeditionOrderRepository;

        public ExpeditionOrderTagShippingService(IExpeditionOrderTagShippingRepository expeditionOrderShippingRepository, IExpeditionOrderRepository expeditionOrderRepository)
        {            
            _expeditionOrderShippingRepository = expeditionOrderShippingRepository;
            _expeditionOrderRepository = expeditionOrderRepository;
        }

        public async Task UpdateShippingOrderIntegraded(string accessKey, ExpeditionOrderTagShipping orderShipping = null)
        {
            if (orderShipping == null)
            {
                orderShipping = await _expeditionOrderShippingRepository.GetShippingByInvoiceAccessKeyAsync(accessKey);
            }

            var completeOrder = await _expeditionOrderRepository.GetOrderByInvoiceAccessKeyAsync(accessKey);

            if (orderShipping != null && completeOrder  != null && (orderShipping.ExpeditionOrderId == 0 || orderShipping.ExpeditionOrderId == null))
            {
                orderShipping.ExpeditionOrderId = completeOrder.Id;
                await _expeditionOrderShippingRepository.UpdateAsync(orderShipping);
            }
        }
    }
}
