using LOGHouseSystem.Adapters.Extensions.Discord;
using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension;
using LOGHouseSystem.Services;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace LOGHouseSystem.Controllers.MVC
{
    public class MelhorEnvioController : Controller
    {        
        private IMelhorEnvioService _melhorEnvioService;
        private IMelhorEnvioAPIServices _melhorEnvioApiService;

        public MelhorEnvioController(IMelhorEnvioService melhorEnvioService, IMelhorEnvioAPIServices melhorEnvioApiService)
        {
            _melhorEnvioService = melhorEnvioService;
            _melhorEnvioApiService = melhorEnvioApiService;
        }
        public async Task<IActionResult> AuthenticationResult(string code, string state)
        {

            var model = new MelhorEnvioActionResultViewModel();

            try
            {                
                await _melhorEnvioApiService.GetAccessNewToken(code, Convert.ToInt32(state));                
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
