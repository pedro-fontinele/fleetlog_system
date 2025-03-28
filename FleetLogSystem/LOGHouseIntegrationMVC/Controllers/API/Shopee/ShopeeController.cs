using LOGHouseSystem.Adapters.Extensions.ShopeeExtension;
using LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto;
using LOGHouseSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.API.Shopee
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopeeController : Controller
    {
        private IShopeeService _shopeeService;
        private IAPIShopeeService _apiShopeeService;

        public ShopeeController(IShopeeService shopeeService, IAPIShopeeService apiShopeeService)
        {
            _shopeeService = shopeeService;
            _apiShopeeService = apiShopeeService;
        }

        [HttpGet("{clientId}/authorization")]
        public ActionResult GetAuthorization([FromRoute] int clientId, [FromQuery] string code, [FromQuery] int main_account_id)
        {            
            try
            {
                // async not applied here because I won't keep the navigator session
                _apiShopeeService.GetAccessNewToken(code, clientId);
            }
            catch (Exception ex)
            {
                Log.Error($"Ocorreu um erro ao ao processar o código de autorização: {(ex.InnerException ?? ex).Message}");
                return BadRequest(ex.Message);
            }
            
            // Close actual Tab after code sended
            return Ok();
        }

        [HttpGet("{clientId}/shipping")]
        public async Task<ActionResult> GetAllShippingFiles([FromRoute] int clientId)
        {
            try
            {
                await _shopeeService.ProcessAllOrderDocuments(clientId);
            }
            catch (Exception ex)
            {
                Log.Error($"Ocorreu um erro ao ao processar o código de autorização: {(ex.InnerException ?? ex).Message}");
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpGet("{clientId}/shipping/{order}")]
        public async Task<ActionResult> GetShippingFile([FromRoute] int clientId, [FromRoute] string order)
        {
            try
            {
                await _shopeeService.DownloadPdfShippingFile(new ShipmentListShiptShopeeDto()
                {
                    OrderNumber = order
                }, clientId);
            }
            catch (Exception ex)
            {
                Log.Error($"Ocorreu um erro ao ao processar o código de autorização: {(ex.InnerException ?? ex).Message}");
                return BadRequest(ex.Message);
            }

            return Ok("Dados processados, verifique o LOG para mais informações");
        }
    }
}
