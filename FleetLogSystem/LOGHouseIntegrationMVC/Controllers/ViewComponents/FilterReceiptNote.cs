using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.ViewComponents
{
    public class FilterReceiptNote : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            FilterViewModel viewModel = new FilterViewModel();

            return View(viewModel);
        }
    }
}
