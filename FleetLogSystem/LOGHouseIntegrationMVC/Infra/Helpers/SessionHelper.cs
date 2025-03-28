using DocumentFormat.OpenXml.InkML;
using LOGHouseSystem.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace LOGHouseSystem.Infra.Helpers
{
    public class SessionHelper
    {
        private HttpContextAccessor _httpContext = new HttpContextAccessor();

        //Busca sessão do usuário
        public User SearchUserSession()
        {
            //string userSession = _httpContext.HttpContext.Session.GetString("userLogedSession");
            string userSessionCookie = _httpContext.HttpContext.Request.Cookies["userId"];
            string userSession = _httpContext.HttpContext.Request.Cookies["user"];

            //if (string.IsNullOrEmpty(userSession)) return null;
            if (string.IsNullOrEmpty(userSessionCookie) || string.IsNullOrEmpty(userSession)) return null;

            return JsonConvert.DeserializeObject<User>(userSession);
        }


        //Cria sessão do usuário
        public void CreateSession(User user)
        {
            string value = JsonConvert.SerializeObject(user);
            _httpContext.HttpContext.Response.Cookies.Append("userId",user.Id.ToString(),new CookieOptions { Expires = DateTimeHelper.GetCurrentDateTime().AddHours(9)});
            _httpContext.HttpContext.Response.Cookies.Append("user", value, new CookieOptions { Expires = DateTimeHelper.GetCurrentDateTime().AddHours(9) });

            //_httpContext.HttpContext.Session.SetString("userLogedSession", value);
        }

        //Remove sessão do usuário
        public void RemoveUserSession()
        {
            //_httpContext.HttpContext.Session.Remove("userLogedSession");
            _httpContext.HttpContext.Response.Cookies.Delete("userId");
            _httpContext.HttpContext.Response.Cookies.Delete("user");
        }
    }
}
