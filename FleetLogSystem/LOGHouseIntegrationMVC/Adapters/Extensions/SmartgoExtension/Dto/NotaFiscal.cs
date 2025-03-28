using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Dto
{
    public class NotaFiscal
    {
        [JsonProperty("numero")]
        public string Numero;

        [JsonProperty("serie")]
        public string Serie;
    }
}
