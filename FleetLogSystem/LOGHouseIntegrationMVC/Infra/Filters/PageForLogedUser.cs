using LOGHouseSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace LOGHouseSystem.Infra.Filters
{
    //Filtro de acesso para usuários logados
    public class PageForLogedUser : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //string userSession = context.HttpContext.Session.GetString("userLogedSession");
            string userSessionCookie = context.HttpContext.Request.Cookies["user"];

            if (string.IsNullOrEmpty(userSessionCookie))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", "Index" } });
            }
            else
            {
                User user = JsonConvert.DeserializeObject<User>(userSessionCookie);

                if (user == null || user.Id <= 0)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", "Index" } });
                }
            }

            //if (string.IsNullOrEmpty(userSessionCookie))
            //{
            //    context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Login" }, { "action", "Index" } });
            //}

            base.OnActionExecuting(context);
        }
    }
}
