using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.ViewComponents
{
    public class FilterDevolution : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            FilterDevolutionViewModel viewModel = new FilterDevolutionViewModel();

            return View(viewModel);
        }
    }
}
