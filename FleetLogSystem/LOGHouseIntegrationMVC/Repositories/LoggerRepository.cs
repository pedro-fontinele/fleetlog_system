using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using PagedList;

namespace LOGHouseSystem.Repositories
{
    public class LoggerRepository : RepositoryBase
    {
        public IQueryable<Logger> GetAlls()
        {
            return _db.Logger;
        }

        public Logger GetById(int id)
        {
            return _db.Logger.Find(id);
        }

        public Logger Add(Logger apiLog)
        {
            _db.Logger.Add(apiLog);
            _db.SaveChanges();

            return apiLog;
        }

        public IPagedList<Logger> GetByDate(DateTime date) { 
          return _db.Logger.Where(x=>x.Date <= date).ToPagedList(1,250);
        }

        public List<Logger> GetByDateAndStatus(DateTime date, LogStatus status)
        {
            return _db.Logger.Where(x => x.Date <= date && x.Status == status).ToList();
        }

        public void DeleteRange(List<Logger> range) {
            _db.Logger.RemoveRange(range);
            _db.SaveChanges();
        }
    }
}
