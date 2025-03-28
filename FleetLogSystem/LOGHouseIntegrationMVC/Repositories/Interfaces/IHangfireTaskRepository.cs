using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IHangfireExecutionRepository
    {
        HangfireExecutionControl Add(HangfireExecutionControl hangfireExecutionControl);

        bool AlreadyRunning(HangfireTask taskType);

        void DeleteByType(HangfireTask taskType);
        
    }
}
