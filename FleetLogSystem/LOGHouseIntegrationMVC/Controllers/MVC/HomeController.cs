using LOGHouseSystem.ViewModels;
using LOGHouseSystem.Infra.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using LOGHouseSystem.Infra.Helpers;

namespace LOGHouseSystem.Controllers.MVC
{
    [PageForLogedUser]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private SessionHelper _session = new SessionHelper();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

       
        public IActionResult Index()
        {
            var user = _session.SearchUserSession();

            if (user != null && user.PermissionLevel != Infra.Enums.PermissionLevel.Admin) 
            { 
                return RedirectToAction("IndexClient");
            }

            DashboardViewModel model = new DashboardViewModel();
            model.TotalStores = 1000;
            model.TotalOrders = 1000;
            return View(model);
        }

        [PageForClient]
        public IActionResult IndexClient()
        {
            return View();
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Login()
        //{
        //    return RedirectToAction("Index");
        //}

    }
}