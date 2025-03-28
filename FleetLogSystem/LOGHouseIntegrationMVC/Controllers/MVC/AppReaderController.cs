using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.MVC
{
    public class AppReaderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
