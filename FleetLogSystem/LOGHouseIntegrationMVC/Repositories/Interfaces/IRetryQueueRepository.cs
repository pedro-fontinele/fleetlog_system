using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IRetryQueueRepository
    {
        public IQueryable<RetryQueue> GetRetryQueues(RetryQueueFilter filters = null);
        public RetryQueue Add(RetryQueue retryQueue);
        public RetryQueue Update(RetryQueue retryQueue);
        public void Delete(RetryQueue retryQueue);
    }
}
