using ClosedXML.Excel;
using Hangfire.MemoryStorage.Database;
using Humanizer;
using LOGHouseSystem.Adapters.Extensions.TinyExtension.Dto.Hook.Request;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace LOGHouseSystem.Controllers.MVC
{
    public class ReportReceiptController : Controller
    {        
        private IReceiptNoteReportService _receiptNoteReportService;
        private readonly IUserRepository _userRepository;


        public ReportReceiptController(IReceiptNoteReportService receiptNoteReportService, IUserRepository userRepository)
        {
            _receiptNoteReportService = receiptNoteReportService;
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            ReceiptNoteReportViewModel viewModel = new ReceiptNoteReportViewModel();
            viewModel.UserLoged = _userRepository.GetUserLoged();
            return View(viewModel);
        }

        public async Task<IActionResult> GenerateReport(ReceiptNoteReportViewModel model)
        {            
            try
            {
                model.UserLoged = _userRepository.GetUserLoged();

                var report = await _receiptNoteReportService.GenerateReport(model);

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = $"Relatorio_Recebimento_{DateTime.Now.ToString()}.xlsx";

                return File(report, contentType, fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction("Index");
            }
        }
    }
}
