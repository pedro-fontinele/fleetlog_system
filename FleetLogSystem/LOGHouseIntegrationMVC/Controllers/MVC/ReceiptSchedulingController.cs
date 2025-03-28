using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Filters;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace LOGHouseSystem.Controllers.MVC
{
    public class ReceiptSchedulingController : Controller
    {
        private readonly IReceiptSchedulingRepository _schedulingRepository;
        private readonly IClientsRepository _clientRepository;
        public readonly IWebHostEnvironment _environment;
        public readonly IConfiguration _configuration;
        private readonly INFeService _nFeService;

        public ReceiptSchedulingController(IReceiptSchedulingRepository schedulingRepository, IClientsRepository clientsRepository, IWebHostEnvironment environment, IConfiguration configuration, INFeService nFeService)
        {
            _schedulingRepository = schedulingRepository;
            _clientRepository = clientsRepository;
            _environment = environment;
            _configuration = configuration;
            _nFeService = nFeService;
        }

        [PageForAdmin]
        public IActionResult Index()
        {
            IEnumerable<ReceiptScheduling> schedules = _schedulingRepository.GetAll();

            return View(schedules);
        }

        [PageForClient]
        public IActionResult SendSchedule()
        {
            return View();
        }

        [PageForClient]
        [HttpPost]
        public IActionResult SendSchedule(IFormFile filePath, ReceiptScheduling scheduling)
        {
            try
            {

                if (!filePath.FileName.Contains(".xml"))
                {
                    throw new Exception($"Erro: O arquivo {filePath.FileName} não é um XML, selecione um arquivo válido.");
                }

                Stream fileStream = filePath.OpenReadStream();

                using var sr = new StreamReader(fileStream, Encoding.UTF8);

                var content = sr.ReadToEnd();

                var note = _nFeService.ValidatingXmlData(content, 0, 1);

                if (!note.Success)
                {
                    throw new Exception($"Não foi possivel realizar a importação. {note.Message}");
                }

                //BytesArray
                MemoryStream target = new MemoryStream();
                filePath.CopyTo(target);
                byte[] data = target.ToArray();

                //Creating path
                string basePath = _environment.WebRootPath;
                var path = Path.Combine(basePath, _configuration["Directories:Files"], filePath.FileName);

                //Savin
                FileHelper.SaveFile(path, data);


                //Setando ID do cliente
                Client client = _clientRepository.GetByUserLoged();
                scheduling.ClientId = client.Id;
                //Setando path
                scheduling.FilesPath = Path.Combine(@"\", filePath.FileName);

                if (ModelState.IsValid)
                {
                    _schedulingRepository.Add(scheduling);
                    TempData["SuccessMessage"] = "Agendamento cadastrado com sucesso";
                    return RedirectToAction("SendSchedule", "ReceiptScheduling");
                }

                
                return View(scheduling);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();

            }
        }




    }
}