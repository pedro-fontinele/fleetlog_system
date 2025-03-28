using LOGHouseSystem.Adapters.Extensions.ShopeeExtension;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Filters;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services;
using LOGHouseSystem.Services.Integrations;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;

namespace LOGHouseSystem.Controllers.MVC
{
    [PageForLogedUser]
    public class IntegrationController : Controller
    {
        
        private readonly IIntegrationRepository _integrationRepository;
        private readonly IIntegrationVariableRepository _integrationVariableRepository;        
        private readonly IClientsRepository _clientsRepository;
        private readonly IIntegrationService _integrationService;
        private readonly IConfiguration _configuration;
        private readonly IAPIShopeeService _aPIShopeeService;

        public IntegrationController(
            IIntegrationRepository integrationRepository, 
            IIntegrationVariableRepository integrationVariableRepository, 
            IIntegrationMercadoLivreService integrationMercadoLivreService, 
            IClientsRepository clientsRepository,
            IIntegrationService integrationService,
            IConfiguration configuration,
            IAPIShopeeService aPIShopeeService)
        {
            _integrationRepository = integrationRepository;
            _integrationVariableRepository = integrationVariableRepository;            
            _clientsRepository = clientsRepository;
            _integrationService = integrationService;
            _configuration = configuration;
            _aPIShopeeService = aPIShopeeService;
        }

        public IActionResult Index()
        {
            IEnumerable<Integration> integrations = _integrationRepository.GetByClientLoged();

            return View(integrations);
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _integrationService.DeleteIntegration(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Create(Integration integration)
        {
            try
            {
                if (ModelState.IsValid)
                {                    
                    bool checkIfIntegrationExists = await _integrationService.CheckIfIntegrationAlreadyExist(integration);

                    if (checkIfIntegrationExists == false)
                    {
                        integration = _integrationService.CreateNewIntegration(integration);

                        TempData["SuccessMessage"] = "Integração criada com sucesso!";
                        return RedirectToAction("Details", "Integration", new { id = integration.Id });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Você já tem uma integração com o {integration.Name}, você não pode configurar uma outra, mas ainda pode editar a sua integração já existente";
                        return RedirectToAction("Index");
                    }
                }

                return View(integration);

            } catch (Exception)
            {
                TempData["ErrorMessage"] = "Não foi possível criar a integração, por favor, tente novamente!";
                return View();
            }
            
        }

        public IActionResult Details(int id)
        {
            try
            {
                var builder = WebApplication.CreateBuilder();

                var app = builder.Build();

                Integration integrationById = _integrationRepository.GetById(id);
                List<IntegrationVariable> variables = _integrationVariableRepository.GetByIntegrationId(integrationById.Id);

                if (integrationById.Name == ShopeeIntegrationNames.IntegrationName)
                {
                    foreach (var item in variables)
                    {
                        if (item.Name == ShopeeIntegrationNames.Url)
                        {
                            item.Value = _aPIShopeeService.GenerateAuthorizationCodeUrlSave(Convert.ToInt32(integrationById.ClientId));
                            break;
                        }
                    }
                }

                Client clientLoged = _clientsRepository.GetByUserLoged();


                if (integrationById.ClientId != clientLoged.Id)
                    return RedirectToAction("Index");

                IntegrationViewModel integrationViewModel = new IntegrationViewModel()
                {
                    Id = integrationById.Id,
                    Name = integrationById.Name,
                    Status = integrationById.Status,
                    Type = integrationById.Type,
                    Variables = variables,
                    UrlToBling = _configuration["Url:UrlValueToDevelopment"] + $"api/BlingCallback/" + clientLoged.Cnpj,
                    UrlToTiny = _configuration["Url:UrlValueToDevelopment"] + "api/TinyHook"

                };

                if (!app.Environment.IsDevelopment())
                {
                    integrationViewModel.UrlToBling = _configuration["Url:UrlValueToProduction"] + $"api/BlingCallback/" + clientLoged.Cnpj;
                    integrationViewModel.UrlToTiny = _configuration["Url:UrlValueToProduction"] + "api/TinyHook?database=prod";
                }

                return View(integrationViewModel);
                
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Não foi possível criar a integração, por favor, tente novamente!";
                return View();
            }

        }

        [HttpPost]
        public IActionResult SaveIntegrationVariableValue([FromBody] SaveIntegrationVariableValueRequest data)
        {
            try
            {
                int id = data.Id;
                string value = data.Value;

                IntegrationVariable integrantionvariableById = _integrationVariableRepository.UpdateValue(id, value);
                
                if(integrantionvariableById != null)
                    return Ok();

                return BadRequest();
            }
            catch
            {
                //TempData["ErrorMessage"] = "Não foi possível atualizar o valor, por favor, tente novamente!";
                return BadRequest("Não foi possível atualizar o valor, por favor, tente novamente!");
            }

        }

    }

    public class SaveIntegrationVariableValueRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
