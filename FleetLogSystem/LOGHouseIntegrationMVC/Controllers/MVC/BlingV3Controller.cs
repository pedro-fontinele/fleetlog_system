using LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.MVC
{
    public class BlingV3Controller : Controller
    {
        private IAPIBlingService _apiBlingService;

        public BlingV3Controller(IAPIBlingService apiBlingService)
        {
            _apiBlingService = apiBlingService;
        }
        public async Task<IActionResult> AuthenticationResult(string code, string state)
        {

            var model = new MelhorEnvioActionResultViewModel();

            try
            {
                if (code == null)
                {
                    throw new Exception("Usuário não authorizado");
                }

                await _apiBlingService.GetAccessNewToken(code, Convert.ToInt32(state));
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
