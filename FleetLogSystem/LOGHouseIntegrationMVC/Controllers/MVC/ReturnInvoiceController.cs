using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.MVC
{
    public class ReturnInvoiceController : Controller
    {

        private readonly IReturnInvoiceService _returnInvoiceService;
        private readonly IExpeditionOrderService _expeditionOrderService;
        private readonly IExpeditionOrderRepository _expeditionOrderRepository;
        private readonly IReturnInvoiceItemRepository _returnInvoiceItemRepository;
        private readonly IExpeditionOrdersLotNotFoundedService _expeditionOrdersLotNotFoundedService;
        private readonly IBlingNFeService _blingNFeService;
        private readonly IReturnInvoiceRepository _returnInvoiceRepository;

        public ReturnInvoiceController(IReturnInvoiceService returnInvoiceService,
            IExpeditionOrderService expeditionOrderService,
            IExpeditionOrderRepository expeditionOrderRepository,
            IReturnInvoiceItemRepository returnInvoiceItemRepository,
            IExpeditionOrdersLotNotFoundedService expeditionOrdersLotNotFoundedService,
            IBlingNFeService blingNFeService,
            IReturnInvoiceRepository returnInvoiceRepository)
        {
            _returnInvoiceService = returnInvoiceService;
            _expeditionOrderService = expeditionOrderService;
            _expeditionOrderRepository = expeditionOrderRepository;
            _returnInvoiceItemRepository = returnInvoiceItemRepository;
            _expeditionOrdersLotNotFoundedService = expeditionOrdersLotNotFoundedService;
            _blingNFeService = blingNFeService;
            _returnInvoiceRepository = returnInvoiceRepository;
        }

        public async Task<IActionResult> NotFounded(NotFoundedPaginationRequestViewModel request)
        {
            request.Status = ExpeditionOrdersLotNotFoundedStatusEnum.Created;

            var ordersNotFounded = await _expeditionOrdersLotNotFoundedService.GetAllNotFoundedOrders(request);

            return View(ordersNotFounded);
        }

        public async Task<IActionResult> NotFoundedSearched(NotFoundedPaginationRequestViewModel request)
        {
            var ordersNotFounded = await _expeditionOrdersLotNotFoundedService.GetAllNotFoundedOrders(request);

            return View("NotFounded", ordersNotFounded);
        }

        [HttpGet("SendInvoice")]
        public async Task<IActionResult> SendInvoice(int returnInvoiceId)
        {
            var error = new Response();

            try
            {
                await _blingNFeService.SendNfeFromReturnInvoice(returnInvoiceId, Convert.ToInt32(Environment.ClientIdLogHouse));

                //error.Error = "A nota de retorno foi enviada com sucesso, confira o status e o detalhamento de emissão.";
            } 
            catch (Exception ex)
            {
                error.Error = string.Format("Não foi possível realizar a emissão da nota fiscal. Detalhes do erro: {0}", ex.Message);                
            }

            return Json(error);
        }

        public async Task<IActionResult> SendNotFounded(int id)
        {
            try
            {
                await _expeditionOrdersLotNotFoundedService.ChangeNotFoundStatus(id, ExpeditionOrdersLotNotFoundedStatusEnum.Pendenting);

                TempData["SuccessMessage"] = "O produto irá ser adicionado ao próximo lote de emissão do cliente.";

                return RedirectToAction("NotFounded");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = string.Format("Não foi possível adicionar o produto ao próximo lote de emissão do cliente. Detalhes do erro: {0}", ex.Message);
                return View("Index");
            }
        }
        
        public async Task<IActionResult> VerifyInvoice(int returnInvoiceId)
        {
            var returnInvoice = await _returnInvoiceRepository.GetByIdAsync(returnInvoiceId);

            return Json(returnInvoice);
        }
        public async Task<IActionResult> GetReturnInvoice(int returnInvoiceId)
        {
            ReturnInvoiceCompleteViewModel returnInvoice = await _returnInvoiceService.GetReturnInvoiceCompleteData(returnInvoiceId);

            return Json(returnInvoice);
        }
        public async Task DeleteXmlAttach(int id)
        {
            await _returnInvoiceService.DeleteAttachedXml(id);
        }

        public async Task<IActionResult> GetInvoicesFromOrder(int expeditionOrderId)
        {
            List<ReturnInvoiceOrdersResponseViewModel> returnInvoice = await _returnInvoiceService.GetReturnInvoicesFromOrder(expeditionOrderId);

            return Json(returnInvoice);
        }

        public async Task<IActionResult> Index(PaginationRequest request)
        {
            PaginationBase<ReturnInvoice> invoices = await _returnInvoiceService.GetAllByPaginationBase(request);
            return View(invoices);
        }

        public async Task<IActionResult> CreateReturnInvoice()
        {
            List<ExpeditionOrderGroup> orders =  await _expeditionOrderService.GetGroupOfOrdersWithIds();
            return View(orders);
        }

        public async Task<IActionResult> UploadXml(List<IFormFile> file, int returnInvoiceId)
        {
            try
            {
                await _returnInvoiceService.AddXmlFile(file, returnInvoiceId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = string.Format("Não foi possível gerar a nota de devolução desse grupo de pedidos, por favor, tente novamente! Detalhes do erro: {0} - {1} - {2}", ex.Message, ex.InnerException, ex.StackTrace);
                return RedirectToAction("Index");
            }
        }        

        public async Task<IActionResult> CreateReturnInvoiceMethod(int clientId) 
        {
            try 
            {

                List<ExpeditionOrder> expeditionOrders = await _expeditionOrderService.GetPendentingReturnOrdersByClientId(clientId);

                if (expeditionOrders.Count() > 0)
                {
                    var invoices = await _returnInvoiceService.AddReturnInvoiceAndItems(expeditionOrders);

                    bool checkResult = invoices != null;

                    var ids = expeditionOrders.Select(e => e.Id).ToList();

                    var lotsNotFounded = await _expeditionOrdersLotNotFoundedService.GetExpeditionOrdersLotNotFoundedByExpeditionOrders(ids);

                    if (lotsNotFounded.Count() > 0)
                    {
                        TempData["SuccessMessage"] = "Não foi possível encontrar o lote de alguns produtos. Verifique as novas pendencias. As notas de devolução foram criadas com sucesso.";
                    }
                    else if(checkResult)
                    {
                        TempData["SuccessMessage"] = "Nota de devolução criada com sucesso!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Não foi possível gerar a nota de devolução desse grupo de pedidos, por favor, tente novamente!";
                    }

                    return RedirectToAction("CreateReturnInvoice");
                }

                TempData["ErrorMessage"] = "Nenhum pedido foi encontrado para geração da nota de retorno.";
                return View("CreateReturnInvoice");

            } catch (Exception ex)
            {
                TempData["ErrorMessage"] = string.Format("Não foi possível gerar a nota de devolução desse grupo de pedidos, por favor, tente novamente! Detalhes do erro: {0} - {1} - {2}", ex.Message, ex.InnerException, ex.StackTrace);
                return View("CreateReturnInvoice");
            }
           
        }

        public async Task<IActionResult> ReturnInvoiceRoutine()
        {
            try
            {
                /*List<List<ExpeditionOrder>> orders = _expeditionOrderService.GetGroupOfOrders();
                if (orders != null)
                {
                    foreach (var listOrder in orders)
                    {
                        bool checkResult = (await _returnInvoiceService.AddReturnInvoiceAndItems(listOrder)) != null;
                        return Ok();
                    }

                }*/

                throw new Exception("Não foi possível criar a nota de retorno.");
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível criar a nota de retorno, detalhe do erro: " + ex);
            }

        }

        public class Response
        {
            public string Error { get; set; } = "";           
        }
    }
}
