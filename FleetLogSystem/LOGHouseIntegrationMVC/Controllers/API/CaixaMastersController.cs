using Microsoft.AspNetCore.Mvc;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Models.API.Requests.CaixaMaster;
using Microsoft.AspNetCore.Authorization;

namespace LOGHouseSystem.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaixaMastersController : ControllerBase
    {
        private readonly ICaixaMastersRepository _caixaMastersRepository;
        private readonly IReceiptNoteItemRepository _receiptNoteItemRepository;


        public CaixaMastersController(ICaixaMastersRepository caixaMastersRepository, IReceiptNoteItemRepository receiptNoteItemRepository)
        {
            _caixaMastersRepository = caixaMastersRepository;
            _receiptNoteItemRepository = receiptNoteItemRepository;
        }

        // POST: api/CaixaMasters
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente,Funcionário")]
        public async Task<ActionResult<CaixaMaster>> PostCaixaMaster(CreateCaixaMasterRequest data)
        {
            var item = _receiptNoteItemRepository.GetById(data.ReceiptNoteItemId);

            if (item == null)
            {
                return Problem("Item de recebimento não encontrado", statusCode: 404);
            }

            if (data.Quantity <= 0)
            {
                return Problem("Quantidade deve ser maior que zero", statusCode: 400);
            }

            if (string.IsNullOrEmpty(data.Code))
            {
                return Problem("Código da caixa master é obrigatório", statusCode: 400);
            }

            if(item.Quantity < data.Quantity)
            {
                return Problem("A quantidade de produtos para recebimento é menor que a quantidade informada", statusCode: 400);
            }

            CaixaMaster caixaMaster = new CaixaMaster
            {
                Code = data.Code,
                CreatedAt = DateTime.Now,
                Quantity = data.Quantity,
                ReceiptNoteItemId = data.ReceiptNoteItemId
            };
                
            caixaMaster = _caixaMastersRepository.Add(caixaMaster);
            return CreatedAtAction("GetCaixaMaster", new { id = caixaMaster.Id }, caixaMaster);
        }

    }
}
