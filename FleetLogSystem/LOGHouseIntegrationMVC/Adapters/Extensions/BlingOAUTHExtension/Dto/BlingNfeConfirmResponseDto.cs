using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto
{
    public class BlingNfeConfirmResponseDto
    {
        [JsonProperty("data")]
        public BlingNfeConfirmDataResponseDto Data { get; set; }
    }

    public class BlingNfeConfirmDataResponseDto
    {
        [JsonProperty("xml")]
        public string Xml { get; set; }
    }
}
