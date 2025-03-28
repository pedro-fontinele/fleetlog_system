using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Dto
{
    public class Deposito
    {
        [JsonProperty("idDeposito")]
        public int IdDeposito;

        [JsonProperty("rastreabilidades")]
        public List<object> Rastreabilidades;
    }
}
