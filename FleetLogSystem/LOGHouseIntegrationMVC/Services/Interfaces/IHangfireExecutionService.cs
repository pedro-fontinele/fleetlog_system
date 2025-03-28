using DocumentFormat.OpenXml.Drawing;
using LOGHouseSystem.Infra.Enums;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IHangfireExecutionService
    {
        bool StartTask(HangfireTask task);

        bool EndTask(HangfireTask task);
    }
}
