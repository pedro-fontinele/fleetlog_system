using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Services.HangFire.Interface;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services.HangFire
{
    public class ShippingTagsRoutine : IShippingTagsRoutine
    {
        private readonly IExpeditionOrderService _expeditionOrderService;
        private readonly IHangfireExecutionService _hangfireExecutionService;

        public ShippingTagsRoutine(IExpeditionOrderService expeditionOrderService,
            IHangfireExecutionService hangfireExecutionService)
        {
            _expeditionOrderService = expeditionOrderService;
            _hangfireExecutionService = hangfireExecutionService;
        }

        public async Task GetBlockedTags()
        {
            if (Environment.EnvironmentName == "Development") return;

            if (!_hangfireExecutionService.StartTask(HangfireTask.GET_BLOCKED_TAGS)) return;

            var orderIdsToProcessTag = _expeditionOrderService.GetTagBlockedOrderIds();

            foreach (int orderId in orderIdsToProcessTag)
            {
                try
                {
                    await _expeditionOrderService.ProcessTag(orderId);
                }
                catch (Exception ex) { }
            }

            _hangfireExecutionService.EndTask(HangfireTask.HOOK_ROUTINE);
        }
    }
}
