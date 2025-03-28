using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LOGHouseSystem.Controllers.ViewComponents
{
    public class PositionAndProduct : ViewComponent
    {
        private readonly IAddressingPositionRepository _addressinPosRepository;

        public PositionAndProduct(IAddressingPositionRepository addressingPositionRepository)
        {
            _addressinPosRepository = addressingPositionRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<AddressingPosition> addP = _addressinPosRepository.GetAll();
            
            return View(addP);
        }
    }
}
