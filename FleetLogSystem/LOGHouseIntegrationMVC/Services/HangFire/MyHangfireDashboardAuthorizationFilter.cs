using Hangfire.Dashboard;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace LOGHouseSystem.Services.HangFire
{
    public class MyHangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {

        private HttpContextAccessor _httpContext = new HttpContextAccessor();

        public bool Authorize(DashboardContext context)
        {
            string userSession = _httpContext.HttpContext.Request.Cookies["user"];

            if (string.IsNullOrEmpty(userSession)) return false;

            User userLoged = JsonConvert.DeserializeObject<User>(userSession);

            if (userLoged.PermissionLevel == PermissionLevel.Admin)
                return true;


            return false;
        }
    }
}
