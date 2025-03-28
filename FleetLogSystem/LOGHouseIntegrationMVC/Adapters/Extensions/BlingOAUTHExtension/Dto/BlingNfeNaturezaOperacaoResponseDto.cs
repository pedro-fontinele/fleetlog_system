using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto
{
    public class BlingNfeNaturezaOperacaoResponseDto
    {
        [JsonProperty("data")]
        public List<BlingNfeNaturezaOperacaoResponseDataDto> Data { get; set; }
    }

    public class BlingNfeNaturezaOperacaoResponseDataDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        [JsonProperty("padrao")]
        public string Padrao { get; set; }

        [JsonProperty("descricao")]
        public string Descricao { get; set; }
    }
}
