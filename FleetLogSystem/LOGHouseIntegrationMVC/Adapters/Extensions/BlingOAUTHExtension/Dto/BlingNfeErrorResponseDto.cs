using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto
{
    public class BlingNfeErrorResponseDto
    {
        [JsonProperty("error")]
        public BlingNfeErrorErrorResponseDto Error { get; set; }
    }

    public class BlingNfeErrorErrorResponseDto
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("fields")]
        public List<BlingNfeErrorFieldResponseDto> Fields { get; set; }
    }

    public class BlingNfeErrorFieldResponseDto
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("element")]
        public string Element { get; set; }

        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        [JsonProperty("collection")]
        public List<BlingNfeErrorCollectionResponseDto> Collection { get; set; }

    }

    public class BlingNfeErrorCollectionResponseDto
    {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("element")]
        public string Element { get; set; }

        [JsonProperty("namespace")]
        public string Namespace { get; set; }
    }
}
