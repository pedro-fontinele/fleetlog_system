using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IRetryQueueService
    {
        public Models.RetryQueue AddHookToRetry(HookInput hookInput);
    }
}
