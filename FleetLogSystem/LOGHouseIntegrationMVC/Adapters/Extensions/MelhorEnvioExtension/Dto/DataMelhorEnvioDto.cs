using LOGHouseSystem.Adapters.Extensions.BaseOAUTH2Extension.Dto;

namespace LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension.Dto
{
    public class DataMelhorEnvioDto : AuthenticationBaseDto
    {
        public int MelhorEnvioClientId { get; set; }
        public string MelhorEnvioSecret { get; set; }
    }
}
