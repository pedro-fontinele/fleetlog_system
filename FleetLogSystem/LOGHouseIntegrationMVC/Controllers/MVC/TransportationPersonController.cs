using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.MVC
{
    public class TransportationPersonController : Controller
    {
        private readonly ITransportationPersonService _transportationPersonService;
        private readonly ITransportationPersonRepository _transportationPersonRepository;

        public TransportationPersonController(ITransportationPersonService transportationPersonService,
            ITransportationPersonRepository transportationPersonRepository)
        {
            _transportationPersonService = transportationPersonService;
            _transportationPersonRepository = transportationPersonRepository;
        }

        public async Task<IActionResult> Index(PaginationRequest request)
        {
            PaginationBase<TransportationPerson> people = await _transportationPersonService.GetPaginationBaseAsync(request);

            return View(people);
        }

        public async Task<IActionResult> Details(int id)
        {
            return View(await _transportationPersonRepository.GetById(id));
        }
    }
}
