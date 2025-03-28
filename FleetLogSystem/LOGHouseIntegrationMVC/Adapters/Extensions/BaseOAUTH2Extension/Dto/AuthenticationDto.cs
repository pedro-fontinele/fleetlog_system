using LOGHouseSystem.Adapters.Extensions.BaseOAUTH2Extension.Dto;

namespace LOGHouseSystem.Adapters.BaseOAUTH2Extension.Dto
{
    public class AuthenticationDto : AuthenticationBaseDto
    {        
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; } // seconds
               
    }
}
