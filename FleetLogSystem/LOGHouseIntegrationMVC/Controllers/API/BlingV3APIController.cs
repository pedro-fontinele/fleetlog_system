using LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension;
using LOGHouseSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]    
    public class BlingV3APIController : ControllerBase
    {        
        private IBlingNFeService _blingNFeService;
        private INFeRoutine _nFeRoutine;

        public BlingV3APIController(IBlingNFeService blingNFeService, INFeRoutine nFeRoutine)
        {
            _blingNFeService = blingNFeService;
            _nFeRoutine = nFeRoutine;
        }

        [HttpGet("{returnInvoiceId}/{clientId}")]
        public async Task<IActionResult> SendNfeByReturnInvoice([FromRoute] int returnInvoiceId, [FromRoute] int clientId)
        {
            await _blingNFeService.SendNfeFromReturnInvoice(returnInvoiceId, clientId);

            return Ok("Analise o LOG para mais informações");
        }

        [HttpGet("routine")]
        public async Task<IActionResult> Routine()
        {
            await _nFeRoutine.Routine();

            return Ok("Analise o LOG para mais informações");
        }
    }
}
