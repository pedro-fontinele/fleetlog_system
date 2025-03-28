using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.ViewComponents
{
    public class FilterPacking : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            FilterPackingViewModel viewModel = new FilterPackingViewModel();

            return View(viewModel);
        }
    }
}
