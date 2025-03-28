using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.MVC
{
    public class ReportExpeditionController : Controller
    {
        private IExpeditionOrderReportService _expeditionOrderReportService;
        private readonly IUserRepository _userRepository;

        public ReportExpeditionController(IExpeditionOrderReportService expeditionOrderReportService, IUserRepository userRepository)
        {
            _expeditionOrderReportService = expeditionOrderReportService;
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            ExpeditionOrderReportViewModel viewModel = new ExpeditionOrderReportViewModel();
            viewModel.UserLoged = _userRepository.GetUserLoged();
            return View(viewModel);
        }

        public async Task<IActionResult> GenerateReport(ExpeditionOrderReportViewModel model)
        {
            try
            {
                model.UserLoged = _userRepository.GetUserLoged();

                var report = await _expeditionOrderReportService.GenerateReport(model);

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = $"Relatorio_Expedicao_{DateTime.Now.ToString()}.xlsx";

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
