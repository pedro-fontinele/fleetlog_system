using LOGHouseSystem.Infra.Enums;

namespace LOGHouseSystem.Models
{
    public class AppVersion
    {
        public int Id { get; set; }

        public string Version { get; set; }

        public ColetorEnum ColetorEnum { get; set; } = 0;

        public string PathString { get; set; }
    }
}
