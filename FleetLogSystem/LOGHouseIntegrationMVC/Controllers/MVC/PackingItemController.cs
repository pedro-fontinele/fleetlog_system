using LOGHouseSystem.Infra.Filters;
using LOGHouseSystem.Models;
using LOGHouseSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.MVC
{
    public class PackingItemController : Controller
    {
        private readonly IPackingItemService _packingItemService;

        public PackingItemController(IPackingItemService packingItemService)
        {
            _packingItemService = packingItemService;
        }

        [PageForAdmin]
        public IActionResult ViewPackingItems(int id)
        {
            List<PackingItem> items = _packingItemService.GetAllByPackingId(id);

            return View(items);
        }
    }
}
