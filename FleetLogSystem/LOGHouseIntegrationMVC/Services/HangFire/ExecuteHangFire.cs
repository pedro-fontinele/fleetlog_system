using Hangfire;
using LOGHouseSystem.Services.HangFire.Interface;

namespace LOGHouseSystem.Services.HangFire
{
    public class ExecuteHangFire
    {
        private readonly IHookInputRoutine _hookInputRoutine;

        public ExecuteHangFire(IHookInputRoutine hookInputRoutine)
        {
            _hookInputRoutine = hookInputRoutine;
        }

        public void Execute()
        {
            RecurringJob.AddOrUpdate<IHookInputRoutine>(x => x.HookRoutine(), "*/20 * * * *");
        }
    }
}
