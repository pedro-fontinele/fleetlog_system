using LOGHouseSystem.Controllers.API.PipedriveHook.Requests;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IPipedriveService _pipedriveService;

        public ClientController(IPipedriveService pipedriveService)
        {
            _pipedriveService = pipedriveService;
        }

        [HttpPost]
        public ActionResult Post([FromBody] List<PipedriveCreateClientRequest> formCollection)
        {

            foreach (var form in formCollection)
            {
                PipedriveCreateClientRequest client = new PipedriveCreateClientRequest
                {
                    Armazenagem = form.Armazenagem,
                    ArmazenagemExcedente = form.ArmazenagemExcedente,
                    ArmazenagemValor = form.ArmazenagemValor,
                    Cnpj = MaskHelper.RemoveMask(form.Cnpj),
                    Email = form.Endereco,
                    Endereco = form.Endereco,
                    Pedidos = form.Pedidos,
                    PedidosValor = form.PedidosValor,
                    RazaoSocial = form.RazaoSocial,
                    Telefone = form.Telefone,
                    UnidadesEnvio = form.UnidadesEnvio,
                    ValorContrato = form.ValorContrato
                };

                _pipedriveService.CreateNewClient(client);

            }
            return Ok();
        }
    }
}
