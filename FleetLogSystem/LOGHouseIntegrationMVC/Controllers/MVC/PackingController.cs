using DocumentFormat.OpenXml.Office2010.Excel;
using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request;
using LOGHouseSystem.Infra.Filters;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using PagedList;

namespace LOGHouseSystem.Controllers.MVC
{
    [PageForLogedUser]
    public class PackingController : Controller
    {
        private readonly IPackingService _packingService;
        private readonly IPackingItemService _packingItemService;
        private readonly IExpeditionOrderService _expeditionOrderService;
        private readonly IPickingListService _pickingListService;
        private readonly IPickingListRepository _pickingListRepository;
        private readonly IPackingHistoryRepository _packingHistoryRepository;
        private readonly IExpeditionOrderHistoryService _expeditionOrderHistoryService;
        private readonly IUserRepository _userRepository;

        public PackingController(IPackingService packingService, 
            IPackingItemService packingItemService, 
            IExpeditionOrderService expeditionOrderService, 
            IPickingListService pickingListService,
            IPackingHistoryRepository packingHistoryRepository,
            IExpeditionOrderHistoryService expeditionOrderHistoryService,
            IUserRepository userRepository,
            IPickingListRepository pickingListRepository)
        {
            _packingService = packingService;
            _packingItemService = packingItemService;
            _expeditionOrderService = expeditionOrderService;
            _pickingListService = pickingListService;
            _packingHistoryRepository = packingHistoryRepository;
            _expeditionOrderHistoryService = expeditionOrderHistoryService;
            _userRepository = userRepository;
            _pickingListRepository = pickingListRepository;
        }

        [PageForAdmin]
        public IActionResult Index(FilterPackingViewModel filter = null)
        {
            PagedList<Packing> list = _packingService.GetByFilter(filter);

            return View(list);
        }

        [PageForAdmin]
        public IActionResult IndexPackingWhithoutPackingListTrasportation(FilterPackingWhithoutPackingListTrasportationViewModel filter = null)
        {
            PagedList<Packing> list = _packingService.GetPackingWhithoutPackingListTrasportationByFilter(filter);

            return View(list);
        }

        [PageForAdmin]
        public IActionResult PackingArea(string InvoiceAccessKey = "", bool cart = false)
        {
            PackingAreaViewModel model = new PackingAreaViewModel
            {
                InvoiceAccessKey = InvoiceAccessKey,
                Cart = cart
            };

            model.Packing = _packingService.SearchByAccessKey(InvoiceAccessKey);

            if (string.IsNullOrEmpty(InvoiceAccessKey))
            {
                return View(model);
            }
            else if(model.Packing == null)
            {
                TempData["ErrorMessage"] = "Ops, ainda não existe empacotamento gerado para esse pedido, portanto, não foi possível encontrá-lo!";
                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult CreatePackingFromPickingList(string[] selecteds)
        {
            try
            {

                //Método para criar o packing com seus itens a partir da expeditionOrder
                List<Packing> packings = _packingService.AddByPickingLists(selecteds);

                if (packings.Count == null)
                {
                    TempData["ErrorMessage"] = "Não foi possível enviar esse item para empacotamento, por favor, tente novamente";
                    return RedirectToAction("Index", "PickingList");
                }

                User userLoged = _userRepository.GetUserLoged();

                foreach (Packing packing in packings)
                {
                    PackingHistory history = new PackingHistory
                    {
                        Observation = "Empacotamento iniciado",
                        Date = DateTimeHelper.GetCurrentDateTime(),
                        Status = Infra.Enums.PackingStatus.Gerado,
                        UserId = userLoged.Id,
                        PackingId = packing.Id
                    };
                    _packingHistoryRepository.AddNotAsync(history);
                    _expeditionOrderHistoryService.AddNotAsync(packing.ExpeditionOrderId, "Empacotamento iniciado", Infra.Enums.ExpeditionOrderStatus.InPacking, userLoged.Id);
                }

                TempData["SuccessMessage"] = "Empacotamento(s) criado(s) com sucesso!";
                return RedirectToAction("Index", "PickingList");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Não foi possível enviar esse item para empacotamento, por favor, tente novamente";
                return RedirectToAction("Index", "PickingList");
            }
        }

        public IActionResult Create(int id)
        {
            try
            {

               //Método para criar o packing com seus itens a partir da expeditionOrder
                Packing packing =  _packingService.AddByOrderExpedition(id, 0);

                User userLoged = _userRepository.GetUserLoged();

                if (packing == null)
                {
                    TempData["ErrorMessage"] = "Não foi possível enviar esse item para empacotamento, por favor, tente novamente";
                    return RedirectToAction("Index", "OrderExpedition");
                }

                PackingHistory history = new PackingHistory
                {
                    Observation = "Empacotamento iniciado",
                    Date = DateTimeHelper.GetCurrentDateTime(),
                    Status = Infra.Enums.PackingStatus.Gerado,
                    UserId = userLoged.Id,
                    PackingId = packing.Id
                };

                _packingHistoryRepository.AddNotAsync(history);

                _expeditionOrderHistoryService.AddNotAsync(packing.ExpeditionOrderId, "Empacotamento iniciado", Infra.Enums.ExpeditionOrderStatus.InPacking, userLoged.Id);

                TempData["SuccessMessage"] = "Empacotamento criado com sucesso!";
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Não foi possível enviar esse item para empacotamento, por favor, tente novamente";
                return RedirectToAction("Index", "OrderExpedition");
            }
        }

        [PageForAdmin]
        [HttpPost]
        public async Task<IActionResult> ImageArea(int packing, string invoiceAccessKey, List<IFormFile> image, string imageBase64)
        {
            try
            {
                await _packingService.SendPackingImage(packing, image, imageBase64);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            PackingAreaViewModel model = new PackingAreaViewModel
            {
                InvoiceAccessKey = invoiceAccessKey
            };

            model.Packing = await _packingService.SearchByAccessKeyAsync(invoiceAccessKey);

            return View("PackingArea", model);
        }


        [PageForAdmin]
        public async Task<IActionResult> ValidateItem(string eanItem, int packing, string invoiceAccessKey)
        {

            try
            {
                ValidatePackingItemRequest request = new ValidatePackingItemRequest()
                {
                    Ean = eanItem,
                    PackingId = packing
                };

                var pickingListItem = await _packingItemService.Validate(request, 0);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;                
            }

            Packing packingModel = _packingService.SearchByAccessKey(invoiceAccessKey);

            User userLoged = _userRepository.GetUserLoged();

            PackingHistory history = new PackingHistory
            {
                Observation = "Empacotamento iniciado",
                Date = DateTimeHelper.GetCurrentDateTime(),
                Status = Infra.Enums.PackingStatus.Gerado,
                UserId = userLoged.Id,
                PackingId = packingModel.Id
            };

            _packingHistoryRepository.AddNotAsync(history);

            _expeditionOrderHistoryService.AddNotAsync(packingModel.ExpeditionOrderId, "Empacotamento iniciado", Infra.Enums.ExpeditionOrderStatus.InPacking, userLoged.Id);

            PackingAreaViewModel model = new PackingAreaViewModel
            {
                InvoiceAccessKey = invoiceAccessKey
            };

            model.Packing = await _packingService.SearchByAccessKeyAsync(invoiceAccessKey);

            return View("PackingArea", model);
        }


        [PageForAdmin]
        public async Task<IActionResult> SetVolumeQuantity(int volumeQuantities, int packing, string invoiceAccessKey)
        {

            try
            {
                var expeditionItem = await _expeditionOrderService.SetVolumeQuantity(invoiceAccessKey, volumeQuantities);    
                

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;                
            }

            PackingAreaViewModel model = new PackingAreaViewModel
            {
                InvoiceAccessKey = invoiceAccessKey
            };

            model.Packing = await _packingService.SearchByAccessKeyAsync(invoiceAccessKey);

            int orderId = model.Packing.ExpeditionOrderId ?? 0;


            return View("PackingArea", model);
        }


        public IActionResult Delete(int id)
        {

            _packingService.DeleteById(id);

            return RedirectToAction("Index", "Packing");
        }

        public IActionResult SearchCart(int cartId)
        {
            PickingList pickinglist = _pickingListService.GetLastByCartId(cartId);

            if (pickinglist == null) return NotFound();

            var response = new
            {
                id = pickinglist.Id,
                expeditionOrder = pickinglist.ExpeditionOrder.Select(eo => new
                {
                    invoiceAccessKey = eo.InvoiceAccessKey,
                    invoiceNumber = eo.InvoiceNumber,
                    clientName = eo.ClientName
                }).ToList(),
                marketplace = pickinglist.MarketPlace.Value.GetDescription(),
                obs = pickinglist.Description

            };

            return Ok(response);
        }

        [HttpGet]
        public async Task<List<PackingHistory>> GetPackingHistoryByPackingId(int packingId)
        {
            List<PackingHistory> histories = await _packingHistoryRepository.GetAllByPackingId(packingId);

            return histories;
        }


    }
}
