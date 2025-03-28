using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension.Dto.Request
{
    public class AuthenticationMelhorEnvioRequestDto
    {
        [JsonProperty("grant_type")]
        public string GrantType { get; set; }

        [JsonProperty("client_id")]        
        public long ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("redirect_uri")]
        public string RedirectUri { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
