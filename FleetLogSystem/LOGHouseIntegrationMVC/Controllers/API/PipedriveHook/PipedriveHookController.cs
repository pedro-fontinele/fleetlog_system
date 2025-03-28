using LOGHouseSystem.Adapters.Extensions.NFeExtension;
using LOGHouseSystem.Controllers.API.PipedriveHook.Requests;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;

namespace LOGHouseSystem.Controllers.API.PipedriveHook
{
    [Route("api/[controller]")]
    [ApiController]
    public class PipedriveHookController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IPipedriveService _pipedriveService;


        public PipedriveHookController(AppDbContext context, IEmailService emailService, IPipedriveService pipedriveService)
        {
            _emailService = emailService;
            _pipedriveService = pipedriveService;
        }

        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public ActionResult Post([FromForm] IFormCollection form)
        {
            //int Armazenagem = int.Parse(form["armazenagem"]);
            //int ArmazenagemExcedente = int.Parse(form["armazenagem-excedente"]);
            //decimal ArmazenagemValor = decimal.Parse(form["armazenagem-valor"]);
            //string Cnpj = form["cnpj"];
            //string Email = form["email"];
            //string Endereco = form["endereco"];
            //int Pedidos = int.Parse(form["pedidos"]);
            //decimal PedidosValor = decimal.Parse(form["pedidos-valor"]);
            //string RazaoSocial = form["razao-social"];
            //string Telefone = form["telefone"];
            //int UnidadesEnvio = int.Parse(form["unidades-envio"]);
            //decimal ValorContrato = decimal.Parse(form["valor-contrato"]);

            PipedriveCreateClientRequest client = new PipedriveCreateClientRequest
            {
                Armazenagem = int.Parse(form["armazenagem"]),
                ArmazenagemExcedente = int.Parse(form["armazenagem-excedente"]),
                ArmazenagemValor = decimal.Parse(form["armazenagem-valor"]),
                Cnpj = MaskHelper.RemoveMask(form["cnpj"]),
                Email = form["email"],
                Endereco = form["endereco"],
                Pedidos = int.Parse(form["pedidos"]),
                PedidosValor = decimal.Parse(form["pedidos-valor"]),
                RazaoSocial = $"{form["razao-social"]}".Replace("+"," "),
                Telefone = form["telefone"],
                UnidadesEnvio = int.Parse(form["unidades-envio"]),
                ValorContrato = decimal.Parse(form["valor-contrato"])
            };

            try
            {
                JObject newClient = JObject.FromObject(client);

                Log.Info($"Novo cliente Pipedrive | Novo cliente recebido no webhook. \n\n Payload: {newClient}");    
            }
            catch(Exception ex)
            {
                throw new Exception($"Detalhe do erro: {ex}");
            }

            
            _pipedriveService.CreateNewClient(client);

            return Ok();
        }
    }
}
