using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension.Dto.Request
{
    public class GetShippingDataMelhorEnvioDtoRequest
    {
        [JsonProperty("mode")]
        public string Mode;

        [JsonProperty("orders")]
        public List<string> Orders;
    }
}
