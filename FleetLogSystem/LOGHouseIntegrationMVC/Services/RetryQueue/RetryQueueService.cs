using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services.RetryQueue
{
    public class RetryQueueService: IRetryQueueService
    {

        private readonly IRetryQueueRepository _repository;

        public RetryQueueService(IRetryQueueRepository repository) { 
        
            _repository = repository;
        }

        public Models.RetryQueue AddHookToRetry(HookInput hookInput)
        {
            Models.RetryQueue retryQueue = new Models.RetryQueue
            {
                HookInputId = hookInput.Id,
                LastTry = DateTimeHelper.GetCurrentDateTime(),
                Tries = 1,
                Note = hookInput.Note
            };

            if (hookInput.Id > 0)
            {
                retryQueue = _repository.GetRetryQueues(new Repositories.RetryQueueFilter
                {
                    HookInputId = hookInput.Id
                }).FirstOrDefault();

                if (retryQueue != null)
                {
                    return retryQueue;
                }
            }
            
            retryQueue = _repository.Add(retryQueue);
            return retryQueue;
        }

    }
}
