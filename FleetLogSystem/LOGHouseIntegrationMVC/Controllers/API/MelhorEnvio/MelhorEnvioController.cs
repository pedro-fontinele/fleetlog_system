using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension;
using LOGHouseSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.API.MelhorEnvio
{
    [Route("api/[controller]")]
    [ApiController]
    public class MelhorEnvioController : Controller
    {
        private IMelhorEnvioService _melhorEnvioService;
        private IMelhorEnvioAPIServices _melhorEnvioApiService;

        public MelhorEnvioController(IMelhorEnvioService melhorEnvioService, IMelhorEnvioAPIServices melhorEnvioApiService)
        {
            _melhorEnvioService = melhorEnvioService;
            _melhorEnvioApiService = melhorEnvioApiService;
        }

        [HttpGet("authorization")]
        public ActionResult GetAuthorization([FromQuery] string code, [FromQuery] string state) {
            try
            {
                // async not applied here because I won't keep the navigator session
                _melhorEnvioApiService.GetAccessNewToken(code, Convert.ToInt32(state));
            }
            catch (Exception ex)
            {
                Log.Error($"Ocorreu um erro ao ao processar o código de autorização: {(ex.InnerException ?? ex).Message}");
                return BadRequest(ex.Message);
            }

            // Close actual Tab after code sended
            return Ok();
        }

        [HttpGet("download/{clientId}")]
        public async Task<ActionResult> GetDownloadShipping([FromRoute] string clientId)
        {
            try
            {
                // async not applied here because I won't keep the navigator session
                await _melhorEnvioService.GetAllReleasedShippingData(Convert.ToInt32(clientId));
            }
            catch (Exception ex)
            {
                Log.Error($"Ocorreu um erro ao ao processar o código de autorização: {(ex.InnerException ?? ex).Message}");
                return BadRequest(ex.Message);
            }

            // Close actual Tab after code sended
            return Ok();
        }
    }
}
