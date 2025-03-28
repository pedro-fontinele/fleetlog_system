using LOGHouseSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog.Layouts;

namespace LOGHouseSystem.Controllers.ViewComponents
{
    public class LayoutControl : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //string userSession = HttpContext.Session.GetString("userLogedSession");
            string userSession = HttpContext.Request.Cookies["user"];

            if (string.IsNullOrEmpty(userSession)) return null;

            User user = JsonConvert.DeserializeObject<User>(userSession);

            if(user.PermissionLevel != Infra.Enums.PermissionLevel.Client)
            {
                ViewBag.Layout = false;
                return View("~/Views/Shared/Components/MenuAdmin/Default.cshtml",user);
            }
            else
            {
                ViewBag.Layout = true;
                return View("~/Views/Shared/Components/MenuClient/Default.cshtml", user);
            }

        }
    }
}
