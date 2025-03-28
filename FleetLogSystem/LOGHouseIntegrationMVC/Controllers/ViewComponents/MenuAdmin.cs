using LOGHouseSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.ViewComponents
{
    public class MenuAdmin : ViewComponent
    {
        //Método para deserializar a sessão, colocando em uma variável e passando pra view "Default" (para mostrar nome do usuário)
        public async Task<IViewComponentResult> InvokeAsync()
        {
            string userSession = HttpContext.Request.Cookies["user"];

            if (string.IsNullOrEmpty(userSession)) return null;

            User user = JsonConvert.DeserializeObject<User>(userSession);

            return View(user);
        }
    }
}
