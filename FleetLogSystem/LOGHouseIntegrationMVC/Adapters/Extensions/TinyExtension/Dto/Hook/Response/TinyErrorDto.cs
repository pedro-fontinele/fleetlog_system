using System.Text.Json.Serialization;

namespace LOGHouseSystem.Adapters.Extensions.TinyExtension.Dto.Hook.Response
{
    public class TinyErrorDto
    {
        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
}
