using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension.Dto.Response
{
    public class GetTagMelhorEnvioResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("protocol")]
        public string Protocol { get; set; }

        [JsonProperty("invoice")]
        public GetTagInvoiceMelhorEnvioResponse Invoice { get; set; }
    }

    public class GetTagInvoiceMelhorEnvioResponse
    {
        [JsonProperty("key")]
        public string InvoiceAccessKey { get; set; }
    }
}
