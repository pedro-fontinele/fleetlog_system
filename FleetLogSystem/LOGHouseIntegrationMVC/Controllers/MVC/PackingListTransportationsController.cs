using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Models;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.Infra.Filters;
using LOGHouseSystem.Services;
using PagedList;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Controllers.MVC
{
    public class PackingListTransportationsController : Controller
    {
       
        private readonly IPackingListTransportationService _packingListTransportationService;
        private readonly IPackingListTransportationRepository _packingListTransportationRepository;

        public PackingListTransportationsController(IPackingListTransportationService packingListTransportationService,
            IPackingListTransportationRepository packingListTransportationRepository)
        {
            _packingListTransportationService = packingListTransportationService;
            _packingListTransportationRepository = packingListTransportationRepository;
        }

        // GET: PackingListTransportations
        [PageForAdmin]
        public IActionResult Index(FilterPackingListTransportationViewModel filter = null)
        {
            PagedList<PackingListTransportation> list = _packingListTransportationService.GetByFilter(filter);
            return View(list);
        }

        // GET: PackingListTransportations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PackingListTransportation? packingListTransportation = _packingListTransportationRepository.GetById((int)id);

            if (packingListTransportation == null)
            {
                return NotFound();
            }

            return View(packingListTransportation);
        }

        [HttpPost]
        [PageForAdmin]
        public JsonResult AutoComplete(string prefix)
        {
            var result = _packingListTransportationRepository.GetIfContainsPrefix(prefix);

            return Json(result);
        }

    }
}
