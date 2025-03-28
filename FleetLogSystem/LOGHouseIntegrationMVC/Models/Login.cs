using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Digite o login")]
        [JsonProperty("loginName")]
        public string LoginName { get; set; }

        [Required(ErrorMessage = "Digite a senha")]
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
