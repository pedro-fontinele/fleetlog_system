using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.Kangu.Dto
{
    public class KanguShippmentResponseDto
    {
        [JsonProperty("pdf")]
        public string Pdf { get; set; }

        [JsonProperty("error")]
        public KanguShippmentErrorResponseDto Error { get; set; }
    }

    public class KanguShippmentErrorResponseDto
    {
        [JsonProperty("codigo")]
        public int Codigo { get; set; }

        [JsonProperty("mensagem")]
        public string Mensagem { get; set; }
    }

    public class KanguShippmentErrorDto
    {
        [JsonProperty("codigo")]
        public int Codigo { get; set; }

        [JsonProperty("Mensagem")]
        public string Mensagem { get; set; }
    }
}
