using LOGHouseSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace LOGHouseSystem.Infra.Filters
{
    public class PageForAdminAndEmployee : ActionFilterAttribute
    {
        //Filtro de acesso para apenas adms
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //string userSession = context.HttpContext.Session.GetString("userLogedSession");
            string userSession = context.HttpContext.Request.Cookies["user"];

            if (string.IsNullOrEmpty(userSession))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login"}, { "action", "Index"} });
            }
            else
            {
                User user = JsonConvert.DeserializeObject<User>(userSession);

                if(user == null)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", "Index" } });
                }

                if(user.PermissionLevel == LOGHouseSystem.Infra.Enums.PermissionLevel.Client )
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "IndexClient" } });
                }
                if (user.PermissionLevel == LOGHouseSystem.Infra.Enums.PermissionLevel.Manager)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "IndexClient" } });
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
