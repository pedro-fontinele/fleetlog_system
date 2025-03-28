using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension;
using LOGHouseSystem.Infra.Filters;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using PagedList;

namespace LOGHouseSystem.Controllers.MVC
{
    public class PickingListController : Controller
    {
        private readonly IPickingListService _pickingListService;
        private readonly IPickingListItemRepository _pickingListItemRepository;
        private readonly IPickingListHistoryService _pickingListHistoryService;
        private readonly IExpeditionOrderService _expeditionOrderService;
        private readonly IExpeditionOrderRepository _expeditionOrderRepository;

        public PickingListController(IPickingListService pickingListService, IPickingListItemRepository pickingListItemRepository, IPickingListHistoryService pickingListHistoryService, IExpeditionOrderService expeditionOrderService, IExpeditionOrderRepository expeditionOrderRepository)
        {
            _pickingListService = pickingListService;
            _pickingListItemRepository = pickingListItemRepository;
            _pickingListHistoryService = pickingListHistoryService;
            _expeditionOrderService = expeditionOrderService;
            _expeditionOrderRepository = expeditionOrderRepository;
        }

        [PageForAdmin]
        public IActionResult Index(int Page = 1, int? CartId = null, int? InvoiceNumber = null)
        {
                PagedList<PickingListWithUrlAndArrayByteViewModel> list = _pickingListService.GetAllPaged(Page, CartId, InvoiceNumber);

                return View(list);
        }

        [PageForAdmin]
        public IActionResult Dashboard()
        {
            List<PickingList> list = _pickingListService.GetWithStatusGeradoAndEmAndamento();

            return View(list);
        }


        [PageForAdmin]
        public IActionResult ViewMore(int id, string routeUrl)
        {
            List<PickingListItem> itemList = _pickingListItemRepository.GetByPickingListId(id);

            PickingListViewMoreViewModel viewModel = new PickingListViewMoreViewModel()
            {
                PickingListItems = itemList,
                RouteUrl = routeUrl
            };

            return View(viewModel);
        }

        [PageForAdmin]
        public async Task<IActionResult> GenerateSimplifiedDanfeList(string[] selecteds)
        {          
            try
            {
                byte[] binaryData = await _pickingListService.GenerateSimplifiedDanfePickingList(selecteds.Select(e => Convert.ToInt32(e)).ToList());

                return File(binaryData, "application/pdf;");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return View("Error", new { RequestId = 0 });
        }


        [PageForAdmin]
        [HttpGet]
        public async Task<List<string>> GenerateMelhorEnvioUrl(string[] selecteds)
        {
            try
            {
                List<ExpeditionOrder> ordersME = await _expeditionOrderRepository.GetAllById(selecteds.Select(e => Convert.ToInt32(e)).ToList());
                List<string> urls = await _expeditionOrderService.GenerateMelhorEnvioUrl(ordersME);

                return urls;
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return null;
            }
        }

        [PageForAdmin]
        [HttpGet]
        public async Task<List<List<string>>> CheckIfOrderIsMelhorEnvio(string[] selecteds)
        {
            try
            {
                return await _pickingListService.CheckIfExistsMelhorEnvioOrder(selecteds.Select(e => Convert.ToInt32(e)).ToList());

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return null;
            }

            
        }



        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                await _pickingListService.Cancel(id);
                TempData["SuccessMessage"] = "Lista de Separação cancelado com sucesso!";
                
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "ERRO: " + e.Message;
                
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<List<PickingListHistory>> GetHistoryByPickingId(int pickingId)
        {

            return await _pickingListHistoryService.GetByPickingId(pickingId);
        }

        public async Task<List<PickingListExpeditionOrdersViewModel>> GetExpeditionOrdersByPickingId(int id)
        {
            List<PickingListExpeditionOrdersViewModel> orders = await _pickingListService.GetOrdersByPickingListId(id);

            return orders;
        }
    }
}
