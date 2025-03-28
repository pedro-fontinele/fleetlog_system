using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.MVC
{
    public class ReceiptNoteLotsController : Controller
    {
        private IReceiptNoteLotsService _receiptNoteLotsService;

        public ReceiptNoteLotsController(IReceiptNoteLotsService receiptNoteLotsService)
        {
            _receiptNoteLotsService = receiptNoteLotsService;
        }
        public async Task<IActionResult> Index(ReceiptNoteIndexLotsPaginationRequest request)
        {
            PaginationBase<ReceiptNoteLots> receiptNoteLots = await _receiptNoteLotsService.GetAllLots(request);
            return View(receiptNoteLots);
        }

        public async Task<IActionResult> Active(int id)
        {
            try
            {
                await _receiptNoteLotsService.ActiveStatus(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }            
        }

        public async Task<IActionResult> Inactive(int id)
        {
            await _receiptNoteLotsService.UpdateStatus(id, LotStatus.Finalizado);
            return RedirectToAction("Index");
        }
    }
}
