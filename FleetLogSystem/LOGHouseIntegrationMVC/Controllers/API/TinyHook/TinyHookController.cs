using LOGHouseSystem.Adapters.Extensions.TinyExtension.Dto.Hook.Request;
using LOGHouseSystem.Adapters.Extensions.TinyExtension.Dto.Hook.Response;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Services;
using LOGHouseSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.API.TinyHook
{

    [Route("api/[controller]")]
    [ApiController]
    public class TinyHookController : ControllerBase
    {        
        private readonly ITinyHookService _tinyHookService;
        private readonly IHookInputService _hookInputService;

        public TinyHookController(AppDbContext context, ITinyHookService tinyHookService, IHookInputService hookInputService)
        {
            _tinyHookService = tinyHookService;
            _hookInputService = hookInputService;
        }

        //[HttpPost]
        //public async Task<ActionResult> Post([FromBody] TinyInvoiceWebhookRequest body)
        //{
        //    string data = JsonConvert.SerializeObject(body);

        //    try
        //    {

        //        Log.Info($"Hook recebido Tiny: {data}");

        //        HookInput hook = await _hookInputService.Add(new HookInput()
        //        {
        //            Cnpj = body.Cnpj,
        //            Payload = data,
        //            Origin = OrderOrigin.Tiny,
        //            Type = HookTypeEnum.Invoice,
        //            Status = true
        //        });

        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error($"Ocorreu um erro ao ao receber pedido do ERP Bling: {(ex.InnerException ?? ex).Message}", new { data = data });
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpPost("urlencoded")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult> PostUrlEncoded([FromForm] IFormCollection form)
        {
            TinyErrorDto error = null;

            try
            {
                var body = new TinyInvoiceWebhookRequest()
                {
                    Cnpj = form["cnpj"],
                    IdEcommerce = int.Parse(form["idEcommerce"]),
                    Tipo = form["tipo"],
                    Versao = form["versao"],
                    Dados = new TinyDataWebhookRequest
                    {
                        ChaveAcesso = form["dados.chaveAcesso"],
                        Numero = Convert.ToInt32(form["dados.numero"]),
                        Serie = int.Parse(form["dados.serie"]),
                        UrlDanfe = form["dados.urlDanfe"],
                        IdPedidoEcommerce = form["dados.idPedidoEcommerce"],
                        DataEmissao = DateTime.Parse(form["dados.dataEmissao"]),
                        ValorNota = float.Parse(form["dados.valorNota"]),
                        IdNotaFiscalTiny = int.Parse(form["dados.idNotaFiscalTiny"])
                    }
                };

                await _tinyHookService.ReceiveOnder(body);
            }
            catch (Exception ex)
            {
                error = new TinyErrorDto()
                {
                    Error = string.Format("{0} - {1}", ex.Message, ex.StackTrace)
                };
            }

            if (error != null)
            {
                Log.Error($"Ocorreu um erro ao ao receber pedido do ERP Tiny (FormCollection): {error.Error}");
                return BadRequest(error);
            }

            return Ok();
        }
    }
}
