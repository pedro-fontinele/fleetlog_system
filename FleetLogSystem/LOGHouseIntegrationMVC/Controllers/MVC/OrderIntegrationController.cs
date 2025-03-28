using LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Services;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.MVC
{
    public class OrderIntegrationController : Controller
    {
        private IBlingService _blingService;

        public OrderIntegrationController(IBlingService blingService)
        {
            _blingService = blingService;
        }
        public IActionResult Index()
        {
            var orders = new OrderIntegrationViewModel();

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> IntegrateOrders(OrderIntegrationViewModel orders)
        {
            try 
            {
                switch(orders.OrderOrigin)
                {
                    case OrderOrigin.Bling:
                        Hangfire.BackgroundJob.Schedule(
                            () => _blingService.IntegrateOrders(orders), TimeSpan.FromSeconds(2));
                        //await _blingService.IntegrateOrders(orders);
                        break;
                    default:
                        throw new Exception("Integração não implemetado");
                }

                TempData["SuccessMessage"] = "Todos os pedidos serão integrados em segundo plano acesse https://fms.loghouse.com.br/hangfire/jobs/processing para acompanhar a tarefa.";

                return View("Index", orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("Index", orders);
            }
        }
    }
}
