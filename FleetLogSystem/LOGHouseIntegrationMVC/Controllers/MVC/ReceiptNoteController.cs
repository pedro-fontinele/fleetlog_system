using LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Dto;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Filters;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.Services.Smartgo;
using LOGHouseSystem.ViewModels;
using LOGHouseSystem.ViewModels.ReceiptNote;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PagedList;
using System.Text;
using System.Xml.Serialization;
using WebSocketSharp;

namespace LOGHouseSystem.Controllers.MVC
{
    
    public class ReceiptNoteController : Controller
    {
        private IReceiptNoteRepository _receiptNoteRepository;
        private IReceiptNoteItemRepository _receiptNoteItemRepository;
        private IProductRepository _productRepository;
        private IPositionAndProductRepository _positionAndProductRepository;
        private readonly IClientsRepository _clientsRepository;
        private readonly ISmartGoService _smartGoService;
        private readonly IReceiptNoteItemService _receiptNoteItemService;
        private readonly ICaixaMastersRepository _caixaMastersRepository;
        private readonly INFeService _nfeService;
        private readonly IDevolutionAndReceiptNoteRepository _devolutionAndReceiptNoteRepository;
        private readonly IDevolutionAndReceiptNoteService _devolutionAndReceiptNoteService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IReceiptSchedulingRepository _receiptSchedulingRepository;
        private readonly IUserRepository _userRepository;

        public ReceiptNoteController(IReceiptNoteItemRepository receiptNoteItemRepository, 
                                     IReceiptNoteRepository receiptNoteRepository, 
                                     IProductRepository productRepository, 
                                     IPositionAndProductRepository positionAndProductRepository, 
                                     IClientsRepository clientsRepository, ISmartGoService smartGoService, 
                                     IReceiptNoteItemService receiptNoteItemService, ICaixaMastersRepository caixaMastersRepository, 
                                     IWebHostEnvironment webHostEnvironment, 
                                     INFeService nfeService, 
                                     IDevolutionAndReceiptNoteRepository devolutionAndReceiptNoteRepository,
                                     IReceiptSchedulingRepository receiptSchedulingRepository,
                                     IDevolutionAndReceiptNoteService devolutionAndReceiptNoteService,
                                     IUserRepository userRepository)
        {
            _receiptNoteItemRepository = receiptNoteItemRepository;
            _receiptNoteRepository = receiptNoteRepository;
            _productRepository = productRepository;
            _positionAndProductRepository = positionAndProductRepository;
            _clientsRepository = clientsRepository;
            _smartGoService = smartGoService;
            _receiptNoteItemService = receiptNoteItemService;
            _caixaMastersRepository = caixaMastersRepository;
            _webHostEnvironment = webHostEnvironment;
            _nfeService = nfeService;
            _devolutionAndReceiptNoteRepository = devolutionAndReceiptNoteRepository;
            _receiptSchedulingRepository = receiptSchedulingRepository;
            _devolutionAndReceiptNoteService = devolutionAndReceiptNoteService;
            _userRepository = userRepository;   
        }

        [PageForAdminAndEmployee]
        public IActionResult IndexAdmin(FilterViewModel filter = null)
        {
            if(!string.IsNullOrEmpty(filter.Cnpj))
            {
                filter.Cnpj = MaskHelper.RemoveMask(filter.Cnpj);

            }
            PagedList<ReceiptNote> list = _receiptNoteRepository.GetByFilters(filter);

            //List<ReceiptNote> notes = _receiptNoteRepository.GetByEntryDate(DateTime.Today);

            return View(list);
        }

        //public IActionResult Filter(FilterViewModel filter)
        //{
        //    filter.Cnpj = MaskHelper.RemoveMask(filter.Cnpj);
        //    List<ReceiptNote> list = _receiptNoteRepository.GetByFilters(filter);

        //    if(list == null || list.Count == 0)
        //    {
        //        TempData["ErrorMessage"] = "Não foi encontrado nenhum valor com os filtros escolhidos, por favor, digite valores válidos.";
        //    }
            
        //    return View("IndexAdmin", list);
        //}

        [PageForClient]
        public IActionResult IndexClient()
        {
            List<ReceiptNote> notes = _receiptNoteRepository.GetByClient();

            return View(notes);
        }

        [PageForLogedUser]
        public IActionResult ViewItems(int id, string routeUrl)
        {
            List<ReceiptNoteItem> noteItens = _receiptNoteItemRepository.GetByReceiptNote(id);
            List< ReceiptNoteItemViewModel > noteVm = new List<ReceiptNoteItemViewModel>();

            foreach (ReceiptNoteItem note in noteItens)
            {
                ReceiptNoteItemViewModel item = new ReceiptNoteItemViewModel()
                {
                    Code = note.Code,
                    Description = note.Description,
                    Ean = note.Ean,
                    Quantity = note.Quantity,
                    QuantityInspection = note.QuantityInspection,
                    Value = note.Value,
                    ItemStatus = note.ItemStatus,
                    ReceiptNoteId = note.ReceiptNoteId,
                    CaixaMasterCode = _caixaMastersRepository.GetCodeByReceiptNoteItemId(note.Id)
                };


                if (item.Ean.ToLower().Contains("sem gtin") || item.Ean.IsNullOrEmpty())
                {
                    item.Ean = item.Code;
                };

                noteVm.Add(item);
            }

            ViewItensViewModel viewModel = new ViewItensViewModel();

            viewModel.UserLoged = _userRepository.GetUserLoged();

            viewModel.Itens = noteVm;

            viewModel.Url = routeUrl;

            return View(viewModel);
        }

        [PageForAdmin]
        public async Task<IActionResult> SetPositionToItems(int id)
        {
            ReceiptNote receiptNote = _receiptNoteRepository.GetById(id);
            List<ReceiptNoteItem> items = receiptNote.ReceiptNoteItems;
            List<Product> products = items.Select(item => _productRepository.GetByEan($"{item.Ean}", receiptNote.ClientId)).ToList();

            List<ReceiptNoteWithPositionViewModel> receiptNoteWithPositionViewModels = await _receiptNoteItemService.GetReceiptNoteItemWithPosition(products);

            for (int i = 0; i < receiptNoteWithPositionViewModels.Count; i++) // Correção aqui
            {
                receiptNoteWithPositionViewModels[i].Quantity = items[i].Quantity;
            }

            return View(new SetPositionToItemsViewModel
            {
                ReceiptNoteID = id,
                Products = receiptNoteWithPositionViewModels
            });
        }

        [PageForAdmin]
        [HttpPost]
        public IActionResult AddPositionProduct(int ReceiptNoteID, string[] positions, string[] productIds)
        {
            try
            {
                 productIds = productIds[0].Split(',');

                List<PositionAndProduct> PositionAndProductsToAdd = new List<PositionAndProduct>();
                ReceiptNote receiptNote = _receiptNoteRepository.GetById(ReceiptNoteID);
                int positionId = 0;

                for (int i = 0; i < productIds.Length; i++)
                {
                    positionId = int.Parse(positions[0]);

                    int productId = int.Parse(productIds[i]);

                    if (_positionAndProductRepository.ProductAlreadyAssociated(positionId, productId)) continue;

                    PositionAndProductsToAdd.Add(new PositionAndProduct
                    {
                        AddressingPositionId = positionId,
                        ProductId = productId
                    });
                }

                if (PositionAndProductsToAdd.Count > 0)
                {
                    _positionAndProductRepository.AddRange(PositionAndProductsToAdd);
                }
                TempData["SuccessMessage"] = "Recebimento finalizado e endereçado com sucesso";
                return RedirectToAction("SetPositionToItems", new {id = ReceiptNoteID});
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Falha ao salvar endereçamento. Detalhes: {ex}";
                return RedirectToAction("SetPositionToItems", new { id = ReceiptNoteID});
            }
           
        }

        [PageForAdmin]
        [HttpPost]
        public async Task<IActionResult> FinalizeReceiptNote(int id)
        {

            try
            {
                ReceiptNote receiptNote = _receiptNoteRepository.GetById(id);

                if(receiptNote.IsDevolution == YesOrNo.Yes)
                {
                    await _devolutionAndReceiptNoteService.FinalizeDevolutionsByNoteIdAnd(id);
                }

                receiptNote.Status = Infra.Enums.NoteStatus.Finalizada;
                foreach (var item in receiptNote.ReceiptNoteItems)
                {
                    item.ItemStatus = Infra.Enums.NoteItemStatus.Finalizado;
                }

                _receiptNoteRepository.Update(receiptNote);

                TempData["SuccessMessage"] = "Recebimento finalizado e endereçado com sucesso";
                return RedirectToAction("Index", "ReceiptNoteDashboard");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Falha ao finalizar recebimento. Detalhes: {ex}";
                return RedirectToAction("SetPositionToItems", new { id = id });
            }
        }



        public async Task<IActionResult> CreateReciptNoteByImportStockIntens(int stockId, int clientId)
        {
            try
            {
                if (clientId <= 0)
                    throw new Exception("Não foi possível achar o cliente informado");

                List<EstoqueSimplificado> list = await _smartGoService.GetSimplifiedStockByDepositorId(stockId);

                Random random = new Random();

                // Gerar 10 números aleatórios e convertê-los em strings
                var numerosAleatorios = Enumerable.Range(1, 10).Select(_ => random.Next(0, 10).ToString());

                // Concatenar os números aleatórios em uma única string
                string numerosConcatenados = string.Join("", numerosAleatorios);

                ReceiptNote receiptNote = new ReceiptNote()
                {
                    Number = clientId.ToString(),
                    SerialNumber = numerosConcatenados.ToString(),
                    AccessKey = numerosConcatenados,
                    EmitDocument = numerosConcatenados,
                    DestDocument = numerosConcatenados,
                    EntryDate = DateTime.Now,
                    IssueDate = DateTime.Now,
                    Status = NoteStatus.Aguardando,
                    ClientId = clientId
                };

                List<ReceiptNoteItem> receiptNoteItemList = new List<ReceiptNoteItem>();

                foreach (var item in list)
                {
                    ReceiptNoteItem noteItem = new ReceiptNoteItem()
                    {
                        Code = item.ProdutoCodigoInterno,
                        Ean = item.ProdutoCodigoExterno,
                        Quantity = item.QuantidadeDisponivel,
                        
                        QuantityInspection = 0,
                        Value = 60,
                        Description = item.ProdutoNome,
                        ReceiptNoteId = receiptNote.Id
                    };

                    receiptNoteItemList.Add(noteItem);
                }
                receiptNote.ReceiptNoteItems = receiptNoteItemList;

                _receiptNoteRepository.Add(receiptNote);

                TempData["SuccessMessage"] = "Nota de recebimento gerada com sucesso!";

                
            } catch (Exception ex)
            {
                TempData["SuccessMessage"] = "Não foi possível gerar a nota de recebimento, erro: " + ex;
                
            }
            return RedirectToAction("Index", "ImportSmartGoStock");
        }

        [PageForAdmin]
        [HttpPost]
        public IActionResult GenerateBarcodeInLabels(string itens)
        {
            try
            {
                var values = JsonConvert.DeserializeObject<List<ReceiptNoteItemTransferViewModel>>(itens);
                byte[] binaryData = _receiptNoteItemService.GeneratePdfWithLabels(values);

                return File(binaryData, "application/pdf;");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Ops! Ocorreu um erro: " + e.Message;
                return RedirectToAction("Index");
            }
        }

        [PageForAdmin]
        [HttpPost]
        public IActionResult GenerateBarcodeToCaixaMasterInLabels(string itens)
        {
            try
            {
                var values = JsonConvert.DeserializeObject<List<CaixaMasterLabelViewModel>>(itens);

                byte[] binaryData = _receiptNoteItemService.GeneratePdfWithCaixMasterLabels(values);

                return File(binaryData, "application/pdf;");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Ops! Ocorreu um erro: " + e.Message;
                return RedirectToAction("Index");
            }
        }

        [PageForAdmin]
        [HttpPost]
        public IActionResult GenerateIdentityBarcodeInLabels(string itens)
        {
            try
            {
                var values = JsonConvert.DeserializeObject<List<CaixaMasterLabelViewModel>>(itens);

                List<string> labelsText = new List<string>();
                foreach (var item in values)
                {
                    string value = item.SKU;
                    labelsText.Add(value);
                }

                byte[] binaryData = _receiptNoteItemService.GeneratePdfWithIdentityLabels(labelsText);

                return File(binaryData, "application/pdf;");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Ops! Ocorreu um erro: " + e.Message; ;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult GenerateNoteBySchenduling(string file, int id)
        {
            try
            {
                if (!string.IsNullOrEmpty(file))
                {
                    // Combine o caminho do arquivo com o diretório raiz do seu aplicativo
                    string diretorioRaiz = _webHostEnvironment.WebRootPath; // WebRootPath é um serviço do ASP.NET Core para obter o diretório raiz do aplicativo.
                    string caminhoCompleto = Path.Combine(diretorioRaiz, "files") + file;

                    if (System.IO.File.Exists(caminhoCompleto))
                    {
                        // Lê o arquivo como um byte array
                        byte[] arquivoBytes = System.IO.File.ReadAllBytes(caminhoCompleto);

                        using (var stream = new MemoryStream(arquivoBytes))
                        {
                            // Crie um objeto IFormFile a partir do MemoryStream
                            var arquivo = new FormFile(stream, 0, stream.Length, "arquivo", Path.GetFileName(caminhoCompleto))
                            {
                                Headers = new HeaderDictionary(),
                                ContentType = "application/octet-stream"
                            };
                            Stream fileStream = arquivo.OpenReadStream();

                            using var sr = new StreamReader(fileStream, Encoding.UTF8);

                            string content = sr.ReadToEnd();


                            //NFE
                            var result = _nfeService.ImportNfe(content, 0, 1);

                            if (result.Success == false)
                            {
                                TempData["ErrorMessage"] = $"{result.Message}";
                                return RedirectToAction("Index", "ReceiptScheduling");
                            }
                        }

                        _receiptSchedulingRepository.UpdateYesOrNoStatus(id, YesOrNo.Yes);


                        TempData["SuccessMessage"] = $"Nota de recebimento criada com sucesso!";
                        return RedirectToAction("Index", "ReceiptScheduling");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"O arquivo XML desse agendamento não foi encontrado ou não existe";
                        return RedirectToAction("Index", "ReceiptScheduling");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = $"O arquivo XML desse agendamento não foi encontrado ou não existe";
                    return RedirectToAction("Index", "ReceiptScheduling");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Ops! Ocorreu um erro: " + e.Message; ;
                return RedirectToAction("Index", "ReceiptScheduling");
            }
        }

        [PageForAdmin]
        [HttpPost]
        public async Task<IActionResult> UploadXmlDevolution(IFormFile xmlFile, string devolutionId)
        {
            try
            {
                List<DevolutionAndReceiptNote> devAndNote = new List<DevolutionAndReceiptNote>();
                var objects = JsonConvert.DeserializeObject<List<dynamic>>(devolutionId);

                // Extrai os valores dos IDs e converte-os em inteiros
                List<int> idList = new List<int>();

                foreach (var obj in objects)
                {
                    int id = Convert.ToInt32(obj.id);
                    idList.Add(id);
                }

                //verifica se existem arquivos 
                if (xmlFile == null || xmlFile.Length == 0)
                {
                    TempData["ErrorMessage"] = "Erro: Corpo do arquivo não encontrado";
                    return RedirectToAction("Index", "Devolution");
                }

                if (!xmlFile.FileName.Contains(".xml"))
                {
                    TempData["ErrorMessage"] = $"Erro: O arquivo {xmlFile.FileName} não é um XML, selecione um arquivo válido.";
                    return RedirectToAction("Index", "Devolution");
                }

                if(devolutionId == "[]")
                {
                    TempData["ErrorMessage"] = "Erro: É necessário selecionar uma devolução antes de fazer o upload do XML";
                    return RedirectToAction("Index", "Devolution");
                }

                Stream fileStream = xmlFile.OpenReadStream();

                using var sr = new StreamReader(fileStream, Encoding.UTF8);

                var content = sr.ReadToEnd();

                //NFE
                var result =  _nfeService.ImportNfe(content, 1);

                if(result.Success == false)
                {
                    TempData["ErrorMessage"] = $"{result.Message}";
                    return RedirectToAction("Index", "Devolution");
                }


                if (result.ReceiptNoteId == 0)
                {
                    TempData["ErrorMessage"] = "Erro: Não foi possível criar a nota de recebimento dessa devolução, por favor, tente novamente!";
                    return RedirectToAction("Index", "Devolution");
                }

                foreach (var id in idList) 
                {
                    DevolutionAndReceiptNote devolutionAndReceiptNote = new DevolutionAndReceiptNote()
                    {
                        DevolutionId = id,
                        ReceiptNoteId = result.ReceiptNoteId
                    };

                    devAndNote.Add(devolutionAndReceiptNote);
                }

                await _devolutionAndReceiptNoteRepository.AddARangesync(devAndNote);

                TempData["SuccessMessage"] = "Nota anexada com sucesso!";

                return RedirectToAction("Index", "Devolution");

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Ops! Ocorreu um erro: " + e.Message; ;
                return RedirectToAction("Index", "Devolution");
            }
        }
    }
}
