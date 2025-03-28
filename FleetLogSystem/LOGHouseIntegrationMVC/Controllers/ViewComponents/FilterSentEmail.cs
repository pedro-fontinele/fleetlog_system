using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.ViewComponents
{
    public class FilterSentEmail : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            FilterSentEmailViewModel viewModel = new FilterSentEmailViewModel();

            return View(viewModel);
        }
    }
}
