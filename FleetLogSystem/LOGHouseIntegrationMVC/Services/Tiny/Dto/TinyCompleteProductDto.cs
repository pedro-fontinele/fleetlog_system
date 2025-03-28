using Newtonsoft.Json;

namespace LOGHouseSystem.Services.Tiny.Dto
{
    public class TinyCompleteProductDto
    {
        [JsonProperty("retorno")]
        public TinyRetornoProductDto Retorno { get; set; }
    }

    public class TinyRetornoProductDto
    {
        [JsonProperty("status_processamento")]
        public long StatusProcessamento { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("produto")]
        public TinyProdutoProductDto Produto { get; set; }
    }

    public class TinyProdutoProductDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }
        public string Gtin { get; set; }

        [JsonProperty("gtin_embalagem")]
        public string GtinEmbalagem { get; set; }
    }
}
