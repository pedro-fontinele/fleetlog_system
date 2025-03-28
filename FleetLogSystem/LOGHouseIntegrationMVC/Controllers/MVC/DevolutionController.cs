using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using LOGHouseSystem.Adapters.Extensions.NFeExtension;
using LOGHouseSystem.Infra.Filters;
using LOGHouseSystem.Models;
using LOGHouseSystem.Models.SendEmails;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using PagedList;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.Web.Helpers;

namespace LOGHouseSystem.Controllers.MVC
{
    public class DevolutionController : Controller
    {

        private readonly IDevolutionService _devolutionService;
        private readonly IDevolutionAndProductRepository _devolutionAndProductRepository;
        private readonly IClientsRepository _clientsRepository;
        private IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DevolutionController(IDevolutionService devolutionService, 
            IDevolutionAndProductRepository devolutionAndProductRepository, 
            IEmailService emailService,
            IClientsRepository clientsRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _devolutionService = devolutionService;
            _devolutionAndProductRepository = devolutionAndProductRepository;
            _emailService = emailService;
            _clientsRepository = clientsRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [PageForAdmin]
        public IActionResult Index(FilterDevolutionViewModel filter = null)
        {
            PagedList<DevolutionCreateAndUpdateViewModel> devolutions = _devolutionService.GetByFilter(filter);

            return View(devolutions);
        }

        [PageForClient]
        public IActionResult IndexClient(int Page = 1)
        {
            PagedList<DevolutionCreateAndUpdateViewModel> devolutions = _devolutionService.GetAllPagedByUserLoged(Page);

            return View(devolutions);
        }

        [PageForClient]
        public async Task<IActionResult> ViewMoreClient(int id)
        {
            List<DevolutionAndProduct> devAndProds = await _devolutionAndProductRepository.GetAllByDevolutionIdAsync(id);

            DevolutionCreateAndUpdateViewModel vm = await _devolutionService.GetViewmodelByIdAsync(id, devAndProds);
            return View(vm);
        }

        public async Task<IActionResult> Create(int? id = 0)
        {

            if(id > 0)
            {
                Devolution devolution = await _devolutionService.GetByIdAsync(id?? 0);

                List<DevolutionAndProduct> devAndProds = await _devolutionAndProductRepository.GetAllByDevolutionIdAsync(id?? 0);

                List<string> prodsName = new List<string>();

                foreach (var dev in devAndProds)
                {
                    prodsName.Add(dev.Product.Description);
                }

                DevolutionCreateAndUpdateViewModel viewModel = new DevolutionCreateAndUpdateViewModel()
                {
                    Id = devolution.Id,
                    SenderName = devolution.SenderName,
                    InvoiceNumber = devolution.InvoiceNumber,
                    PostNumber = devolution.PostNumber,
                    Observation = devolution.Observation,
                    Status = devolution.Status,
                    ClientId = devolution.ClientId,
                    ClientName = devolution.Client.SocialReason,
                    Products = prodsName,
                    IsCreation = false
                };

                return View(viewModel);
            }
            return View();
        }

        [HttpPost]
        [PageForAdmin]
        public async Task<IActionResult> Create(DevolutionCreateAndUpdateViewModel devolution, string productsId)
        {
            try
            {
                if (devolution.IsCreation)
                {
                    Devolution dev = await _devolutionService.AddAsync(devolution);
                    dev.Client = _clientsRepository.GetById(devolution.ClientId);

                    List<int> productsIdList = _devolutionService.GetProductsIdList(productsId);

                    List<DevolutionAndProduct> devAndProdList = await _devolutionAndProductRepository.AddAllAsync(dev.Id, productsIdList);

                    //_devolutionService.SendDevolutionEmail(dev, devAndProdList, null);

                    TempData["SuccessMessage"] = "Devolução criada com sucesso! Agora adicione as fotos dos produtos devolvidos, caso necessário";

                    return RedirectToAction("UploadImages", new { id = dev.Id });
                }
                else
                {
                    Devolution devolutionUpdated = await _devolutionService.UpdateAsync(devolution);

                    if(productsId != "[]")
                    {
                        List<int> productsIdList = _devolutionService.GetProductsIdList(productsId);

                        await _devolutionAndProductRepository.AddAllAsync(devolutionUpdated.Id, productsIdList);

                    }

                    TempData["SuccessMessage"] = "Devolução atualizada com sucesso! Agora adicione as fotos dos produtos devolvidos, caso necessário";

                    return RedirectToAction("UploadImages", new { id = devolutionUpdated.Id });
                }

            } catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Não foi possível realizar esta ação, por favor, verifique as informações preenchidas e tente novamente!";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> UploadImages(int id)
        {

            List<DevolutionAndProduct> devAndProds = await _devolutionAndProductRepository.GetAllByDevolutionIdAsync(id);

            DevolutionAndProductsViewModel viewModel = new DevolutionAndProductsViewModel();
            List<Product> productList = new List<Product>();
            List<DevolutionImage> images = await _devolutionService.GetAllImagesByIdAsync(id);

            foreach (var dev in devAndProds)
            {
                productList.Add(dev.Product);
            }

            if(images.Count > 0)
                viewModel.Images = images;  

            viewModel.DevolutionId = id;
            viewModel.Products = productList;

            return View(viewModel);
        }

        [PageForAdmin]
        [HttpPost]
        public async Task<IActionResult> ImageArea(int devolutionId, List<IFormFile> image, string imageBase64)
        {
            try
            {
                await _devolutionService.SendDevolutionImage(devolutionId, image, imageBase64);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            TempData["SuccessMessage"] = "Foto adicionada a devolução com sucesso!";
            return RedirectToAction("UploadImages", new { id = devolutionId});
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int devolutionId)
        {
            try
            {
                bool result = await _devolutionService.DeleteAsync(devolutionId);

                if(result)
                {
                    TempData["SuccessMessage"] = "Devolução apagada com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Ops! Não foi possível apagar essa devolução, por favor, tente novamente!";
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        
        public async Task<IActionResult> FinalizeDevolution(int devolutionId)
        {
            try
            {
                List<DevolutionImage> images = await _devolutionService.GetAllImagesByIdAsync(devolutionId);

                // Crie uma lista de anexos
                List<byte[]> imageBytesList = new List<byte[]>();

                foreach (DevolutionImage image in images)
                {
                    string relativePath = image.FilePath.TrimStart('~'); // Remova o caractere "~" do início
                    string diretorioRaiz = _webHostEnvironment.WebRootPath;
                    string absolutePath = Path.Combine(diretorioRaiz) + relativePath;

                    if (System.IO.File.Exists(absolutePath))
                    {
                        // Lê o arquivo como um array de bytes
                        byte[] imageBytes = System.IO.File.ReadAllBytes(absolutePath);
                        imageBytesList.Add(imageBytes);
                    }
                }

                Devolution dev = await _devolutionService.GetByIdAsync(devolutionId);
                List<DevolutionAndProduct> devAndProds = await _devolutionAndProductRepository.GetAllByDevolutionIdAsync(devolutionId);

                _devolutionService.SendDevolutionEmail(dev, devAndProds, imageBytesList);

                TempData["SuccessMessage"] = "Devolução finalizada com sucesso! Foi enviado um email ao cliente com as informações dessa devolução.";

                return RedirectToAction("Index");
                
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
