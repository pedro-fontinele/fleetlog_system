using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;

namespace LOGHouseSystem.Repositories
{
    public class HangfireExecutionRepository : RepositoryBase, IHangfireExecutionRepository
    {
        public HangfireExecutionControl Add(HangfireExecutionControl hangfireExecutionControl)
        {
            _db.HangfireExecutionControls.Add(hangfireExecutionControl);
            _db.SaveChanges();
            return hangfireExecutionControl;
        }

        public bool AlreadyRunning(HangfireTask taskType)
        {
            var task = _db.HangfireExecutionControls.Where(he => he.Task == taskType).FirstOrDefault();
            if(task != null)
            {
                if (task.StartDate < DateTimeHelper.GetCurrentDateTime().AddMinutes(-20))
                {
                    task = null;
                    DeleteByType(taskType);
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public void DeleteByType(HangfireTask taskType)
        {
            var hes = _db.HangfireExecutionControls.Where(he => he.Task == taskType).ToList();

            _db.HangfireExecutionControls.RemoveRange(hes);
            _db.SaveChanges();
        }
    }
}
