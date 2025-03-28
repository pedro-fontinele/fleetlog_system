using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.Kangu.Dto
{
    public class KanguPostTagRequestDto
    {
        [JsonProperty("modelo")]
        public string Modelo { get; set; }

        [JsonProperty("codigo")]
        public List<string> Codigo { get; set; }
    }
}
