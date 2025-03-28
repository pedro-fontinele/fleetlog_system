using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension.Dto.Response
{
    public class GetAllShippingMelhorEnvioDtoResponse
    {
        [JsonProperty("current_page")]
        public int CurrentPage { get; set; }

        [JsonProperty("data")]
        public List<OrderShippingMelhorEnvioDto> Data { get; set; }
    }

    public class OrderShippingMelhorEnvioDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("reminder")]
        public string Reminder { get; set; }

        [JsonProperty("tags")]
        public List<OrderShippingTagMelhorEnvioDto> Tags { get; set; }

        [JsonProperty("invoice")]
        public OrderShippingInvoiceMelhorEnvioDto Invoice { get; set; }
    }

    public class OrderShippingTagMelhorEnvioDto
    {
        [JsonProperty("tag")]
        public string? Tag { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }
    }

    public class OrderShippingInvoiceMelhorEnvioDto
    {
        [JsonProperty("key")]
        public string? Key { get; set; }
    }
}
