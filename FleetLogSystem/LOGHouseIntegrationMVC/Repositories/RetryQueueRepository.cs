using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;

namespace LOGHouseSystem.Repositories
{
    public class RetryQueueFilter
    {
        public DateTime? EndDate { get; set; }

        public int? MaxTries { get; set; }

        public int? HookInputId { get; set; }
    }

    public class RetryQueueRepository : RepositoryBase, IRetryQueueRepository
    {
        public IQueryable<RetryQueue> GetRetryQueues(RetryQueueFilter filters = null)
        {
            var query = _db.RetryQueues.Select(rq => rq);

            if(filters != null)
            {
                if(filters.MaxTries > 0) query = query.Where(rq=>rq.Tries < filters.MaxTries);
                if(filters.HookInputId > 0) query = query.Where(rq=>rq.HookInputId == filters.HookInputId);
                if(filters.EndDate != null) query = query.Where(rq=>rq.LastTry <= filters.EndDate);
            }

            return query;
        }

        public RetryQueue Add(RetryQueue retryQueue)
        {
            _db.RetryQueues.Add(retryQueue);
            _db.SaveChanges();
            return retryQueue;
        }

        public void Delete(RetryQueue retryQueue)
        {
            _db.RetryQueues.Remove(retryQueue);
            _db.SaveChanges();
        }

        public RetryQueue Update(RetryQueue retryQueue)
        {
            _db.Entry(retryQueue).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _db.SaveChanges();

            return retryQueue;
        }
    }
}
