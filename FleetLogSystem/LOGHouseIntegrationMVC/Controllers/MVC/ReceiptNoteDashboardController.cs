
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Filters;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Services;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;

namespace LOGHouseSystem.Controllers.MVC
{
    public class ReceiptNoteDashboardController : Controller
    {

        public IReceiptNoteService _receiptNoteService;
        public ReceiptNoteDashboardController(IReceiptNoteService receiptNoteService )
        {
            _receiptNoteService = receiptNoteService;
        }

        [PageForAdmin]
        public IActionResult Index()
        {
            return View(_receiptNoteService.ReceiptNoteToDashboard());
        }

        //[PageForLogedUser]
        public IActionResult Confirm(int id)
        {
            _receiptNoteService.ConfirmReceiptNote(id);

            return RedirectToAction("SetPositionToItems", "ReceiptNote", new {id = id});
        }

        //[PageForLogedUser]
        public IActionResult Reject(int id, string subject, int route, string? message)
        {
            _receiptNoteService.RejectReceiptNote(id, subject, message);
            TempData["SuccessMessage"] = "Nota de Recebimento rejeitada com sucesso!";

            if(route == 0)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("IndexAdmin", "ReceiptNote");

        }
        
        //[PageForLogedUser]
        public IActionResult Reset(int id)
        {
            _receiptNoteService.ResetReceiptNote(id);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            _receiptNoteService.DeleteById(id);

            return RedirectToAction("Index");
        }
    }
}
