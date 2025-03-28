using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.ViewComponents
{
    public class FilterPackingListTransportation : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            FilterPackingListTransportationViewModel viewModel = new FilterPackingListTransportationViewModel();

            return View(viewModel);
        }
    }
}
