using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Integrations;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.MVC
{
    public class AuthMLController : Controller
    {
        private readonly IIntegrationMercadoLivreService _integrationMercadoLivreService;

        public AuthMLController(IIntegrationRepository integrationRepository, IIntegrationVariableRepository integrationVariableRepository, IIntegrationMercadoLivreService integrationMercadoLivreService)
        {
            _integrationMercadoLivreService = integrationMercadoLivreService;
        }

        public async Task<IActionResult> Index(string code, string state)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
                return NotFound();

            int integrationId = Int32.Parse(state);
            try
            {
                await _integrationMercadoLivreService.ConfigMLAccessToken(integrationId, code);
            }catch(Exception ex)
            {
                return View(new { error = ex.Message });
            }

            return View(new { success = true });
        }
    }
}
