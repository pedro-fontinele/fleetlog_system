using LOGHouseSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace LOGHouseSystem.Infra.Filters
{
    //Filtro de acesso para usuários logados
    public class PageForClient : ActionFilterAttribute
    {
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
                if (user.PermissionLevel != LOGHouseSystem.Infra.Enums.PermissionLevel.Client)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
