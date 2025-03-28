using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services
{
    public class HangfireExecutionService : IHangfireExecutionService
    {
        private IHangfireExecutionRepository _hangfireExecutionRepository;
        public HangfireExecutionService(IHangfireExecutionRepository hangfireExecutionRepository) 
        {
            _hangfireExecutionRepository = hangfireExecutionRepository;
        }

        public bool StartTask(HangfireTask task)
        {
            if(_hangfireExecutionRepository.AlreadyRunning(task)) return false;
            else
            {
                _hangfireExecutionRepository.Add(new Models.HangfireExecutionControl { StartDate = DateTimeHelper.GetCurrentDateTime(), Task = task });
            }

            return true;
        }

        public bool EndTask(HangfireTask task)
        {
            if (!_hangfireExecutionRepository.AlreadyRunning(task)) return true;
            else
            {
                _hangfireExecutionRepository.DeleteByType(task);
            }

            return true;
        }
    }
}
