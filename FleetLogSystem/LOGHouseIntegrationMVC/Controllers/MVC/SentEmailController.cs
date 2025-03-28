using LOGHouseSystem.Infra.Filters;
using LOGHouseSystem.Models;
using LOGHouseSystem.Services;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using PagedList;

namespace LOGHouseSystem.Controllers.MVC
{
    [PageForAdmin]
    public class SentEmailController : Controller
    {
        private readonly ISentEmailService _sentEmailService;

        public SentEmailController(ISentEmailService sentEmailService)
        {
            _sentEmailService = sentEmailService;
        }

        
        public async Task<IActionResult> Index(FilterSentEmailViewModel filter = null)
        {
            PagedList<SentEmailViewModel> emails = _sentEmailService.GetByFilter(filter);
            return View(emails);
        }

        public async Task<IActionResult> EmailsReceivedByClient(int id)
        {
            List<SentEmail> emails = await _sentEmailService.GetSentEmailsByClientId(id);
            return View(emails);
        }
    }
}
