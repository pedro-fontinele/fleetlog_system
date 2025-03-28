using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto
{
    public class BlingNfeResponseDto
    {
        [JsonProperty("data")]
        public BlingNfeDataResponseDto Data { get; set; }
    }

    public partial class BlingNfeDataResponseDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("numero")]
        public long Numero { get; set; }

        [JsonProperty("serie")]
        public long Serie { get; set; }

        [JsonProperty("contato")]
        public BlingNfeContatoResponseDto Contato { get; set; }
    }

    public partial class BlingNfeContatoResponseDto
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }
    }
}
