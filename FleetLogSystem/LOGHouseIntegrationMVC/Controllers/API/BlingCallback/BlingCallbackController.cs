using LOGHouseSystem.Adapters.Extensions.BlingExtension.Dto.Hook.Request;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Services;
using LOGHouseSystem.Services.HangFire.Interface;
using LOGHouseSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Web;

namespace LOGHouseSystem.Controllers.API.BlingHook
{

    [Route("api/[controller]")]
    [ApiController]
    public class BlingCallbackController : ControllerBase
    {
        private IBlingCallbackService _blingCallbackService;
        private IHookInputService _hookInputService;
        private IHookInputRoutine _hookInputRoutine;
        private IRefreshTokenRoutine _refreshTokenRoutine;

        public BlingCallbackController(IBlingCallbackService blingCallbackService, IHookInputService hookInputService, IHookInputRoutine hookInputRoutine, IRefreshTokenRoutine refreshTokenRoutine)
        {
            _blingCallbackService = blingCallbackService;
            _hookInputService = hookInputService;
            _hookInputRoutine = hookInputRoutine;
            _refreshTokenRoutine = refreshTokenRoutine;
        }

        //Receive Orders
        //[HttpPost("{cnpj}")]        
        //public async Task<ActionResult> OrderBlingCallback([FromForm] string data, [FromRoute] string cnpj)
        //{
        //    try
        //    {

        //        Log.Info($"Callback recebido Bling: {data}");

        //        HookInput hook = await _hookInputService.Add(data, OrderOrigin.Bling, cnpj);

        //       return Ok();

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error($"Ocorreu um erro ao ao receber pedido do ERP Bling: {(ex.InnerException ?? ex).Message}",new {data});
        //        return BadRequest(ex.Message);
        //    }           
        //}

        //Receive Orders
        [HttpPost("{cnpj}/direct")]
        public async Task<ActionResult> OrderBlingCallbackDirect([FromForm] string data, [FromRoute] string cnpj)
        {
            try
            {

                Log.Info($"Callback recebido Bling direto: {data}");

                await _blingCallbackService.ProcessBlingOrder(new HookInput() { Cnpj = cnpj, Payload = data, Origin = OrderOrigin.Bling });
                
                return Ok();

            }
            catch (Exception ex)
            {
                Log.Error($"Ocorreu um erro ao ao receber pedido do ERP Bling: {(ex.InnerException ?? ex).Message}", new { data });
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("forcehook")]
        public async Task<ActionResult> ForceHook()
        {

            await _hookInputRoutine.HookRoutine();

            return Ok();
        }

        [HttpGet("forcehookinvoice")]
        public async Task<ActionResult> ForceHookInvoice()
        {

            await _hookInputRoutine.HookInvoiceRoutine();

            return Ok();
        }

        [HttpGet("forcerefresh")]
        public async Task<ActionResult> ForceRefresh()
        {

            await _refreshTokenRoutine.RefreshAccessTokensRoutine();

            return Ok();
        }


        [HttpPost]
        public async Task<ActionResult> OrderBlingCallback([FromBody] BlingCallbackRequestDto data)
        {           

            return BadRequest("Utilize a rota /api/BlingCallback/[CNPJ] para cadastrar esse callback");
        }

    }
}
