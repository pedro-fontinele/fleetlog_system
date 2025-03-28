using LOGHouseSystem.Models;
using LOGHouseSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LOGHouseSystem.Controllers.MVC
{
    public class ShippingCompanyController : Controller
    {

        private readonly IShippingCompanyService _shippingCompanyService;

        public ShippingCompanyController(IShippingCompanyService shippingCompanyService)
        {
            _shippingCompanyService = shippingCompanyService;
        }

        public async Task<IActionResult> Index()
        {
            List<ShippingCompany> companies = await _shippingCompanyService.GetAll();

            return View(companies);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ShippingCompany shippingCompany)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    _shippingCompanyService.AddCompany(shippingCompany);
                    TempData["SuccessMessage"] = "Transportadora cadastrada com sucesso!";
                    return RedirectToAction("Index");
                }
                return View(shippingCompany);

            } catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Não foi possível cadastrar a transportadora, por favor, tente novamente! Detalhes do erro: {ex}";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            ShippingCompany company = await _shippingCompanyService.GetById(id);

            return View(company);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ShippingCompany shippingCompany)
        {
            try
            {
                if (ModelState.IsValid) 
                {
                   await _shippingCompanyService.UpdateCompany(shippingCompany);
                   TempData["SuccessMessage"] = "A Transportadora foi atualizada com sucesso!";
                   return RedirectToAction("Index");
                }

                return View(shippingCompany);

            }catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Não foi possível atualizar a transportadora, por favor, tente novamente! Detalhes do erro: {ex}";
                return RedirectToAction("Index");
            }

        }
    }
}
