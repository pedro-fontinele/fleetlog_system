using LOGHouseSystem.Adapters.Extensions.ShopeeExtension;
using LOGHouseSystem.Services;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.MVC
{
    public class ShopeeController : Controller
    {
        private IAPIShopeeService _apiShopeeService;
        private IShopeeService _shopeeService;

        public ShopeeController(IAPIShopeeService apiShopeeService, IShopeeService shopeeService)
        {
            _apiShopeeService = apiShopeeService;
            _shopeeService = shopeeService;
        }
        public async Task<IActionResult> AuthenticationResult(string code, string main_account_id, string clientId)
        {
          var model = new MelhorEnvioActionResultViewModel();
            try
            {
                if (code == null)
                {
                    throw new Exception("Usuário não authorizado");
                }
                
                await _apiShopeeService.GetAccessNewToken(code, Convert.ToInt32(clientId));
                //_shopeeService.ProcessAllOrderDocuments(Convert.ToInt32(clientId));
            }
            catch (Exception ex)
            {
                Log.Error($"Ocorreu um erro ao ao processar o código de autorização: {(ex.InnerException ?? ex).Message}");                
                model.ErrorMessage = $"Ocorreu um erro ao ao processar o código de autorização: {(ex.InnerException ?? ex).Message}";
            }

            return View("AuthenticationResult", model);
        }
    }
}
