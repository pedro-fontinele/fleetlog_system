using LOGHouseSystem.Infra.Enums;

namespace LOGHouseSystem.Models
{
    public class HangfireExecutionControl
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public HangfireTask Task { get; set; }
    }
}
