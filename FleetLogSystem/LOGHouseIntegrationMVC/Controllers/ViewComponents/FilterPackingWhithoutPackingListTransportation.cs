using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.ViewComponents
{
    public class FilterPackingWhithoutPackingListTransportation : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            FilterPackingWhithoutPackingListTrasportationViewModel viewModel = new FilterPackingWhithoutPackingListTrasportationViewModel();

            return View(viewModel);
        }
    }
}
