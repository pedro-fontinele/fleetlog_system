﻿using Newtonsoft.Json;

namespace LOGHouseSystem.Adapters.BaseOAUTH2Extension.Dto
{
    public class AuthenticationAPIResponseDto
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}
