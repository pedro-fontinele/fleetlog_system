using DocumentFormat.OpenXml.Drawing.Charts;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Filters;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Infra.Helpers.CustomExceptions;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Globalization;
using System.Text;

namespace LOGHouseSystem.Controllers.MVC
{
    [PageForLogedUser]
    public class ExpeditionOrderController : Controller
    {
        private readonly IExpeditionOrderRepository _expeditionOrderRepository;
        private readonly IExpeditionOrderItemsRepository _expeditionOrderItemsRepository;
        private readonly IShippingDetailsRepository _shippingDetailsRepository;
        private readonly IClientsRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IExpeditionOrderService _expeditionOrderService;
        private readonly INFeService _nfeService;
        private readonly IExpeditionOrderTagShippingRepository _expeditionOrderTagShippingRepository;
        private readonly IUserRepository _userRepository;
        private SessionHelper _session = new SessionHelper();
        private readonly IPickingListHistoryService _pickingListHistoryService;
        private readonly IExpeditionOrderHistoryService _expeditionOrderHistoryService;
        private readonly IPackingListTransportationRepository _packingListTransportationRepository;
        private readonly IPackingRepository _packingRepository;

        public ExpeditionOrderController(IExpeditionOrderRepository expeditionOrderRepository,
            IShippingDetailsRepository shippingDetailsRepository,
            IExpeditionOrderService expeditionOrderService,
            IClientsRepository clientRepository,
            IExpeditionOrderItemsRepository expeditionOrderItemsRepository,
            IProductRepository productRepository,
            INFeService nfeService,
            IExpeditionOrderTagShippingRepository expeditionOrderTagShippingRepository,
            IUserRepository userRepository,
            IPickingListHistoryService pickingListHistoryService,
            IExpeditionOrderHistoryService expeditionOrderHistoryService,
            IPackingListTransportationRepository packingListTransportationRepository,
            IPackingRepository packingRepository
            )
        {
            _expeditionOrderRepository = expeditionOrderRepository;
            _shippingDetailsRepository = shippingDetailsRepository;
            _clientRepository = clientRepository;
            _expeditionOrderItemsRepository = expeditionOrderItemsRepository;
            _productRepository = productRepository;
            _expeditionOrderService = expeditionOrderService;
            _nfeService = nfeService;
            _expeditionOrderTagShippingRepository = expeditionOrderTagShippingRepository;
            _userRepository = userRepository;
            _pickingListHistoryService = pickingListHistoryService;
            _expeditionOrderHistoryService = expeditionOrderHistoryService;
            _packingListTransportationRepository = packingListTransportationRepository;
            _packingRepository = packingRepository;
        }

        public async Task<IActionResult> Index()
        {
            ExpeditionOrderFilterViewModel request = new ExpeditionOrderFilterViewModel();
            request.PageNumber = 1;


            ExpeditionOrderIndexViewModel reponseVwa = new ExpeditionOrderIndexViewModel();

            return await RenderOrders(ExpeditionOrdersPage.Integration, reponseVwa, request);

        }

        public async Task<IActionResult> IndexManualOrders(ExpeditionOrderFilterViewModel filter = null)
        {

            filter.PageNumber = 1;
            PaginationBase <ExpeditionOrderWithPickingListViewModel> paginationOrders =  await _expeditionOrderService.GetAllManualOrders(filter);

            ExpeditionOrderIndexViewModel response = new ExpeditionOrderIndexViewModel();

            response.Orders = paginationOrders;
            response.Filter = new ExpeditionOrderFilterViewModel();
            response.Filter = filter;
            response.Page = ExpeditionOrdersPage.Orders;
            response.UserLoged = _userRepository.GetUserLoged();
            response.TotalPages = paginationOrders.TotalPages;
            response.PageNumber = paginationOrders.PageNumber;
            return View(response);
        }


        public IActionResult CreateByXML()
        {
            return View();
        }


        public IActionResult ViewMoreIntegration(int id)
        {
            ExpeditionOrder expeditionOrder = _expeditionOrderRepository.GetById(id);

            return View(expeditionOrder);
        }


        public IActionResult ViewMoreOrders(int id, string? routeUrl)
        {
            ExpeditionOrder expeditionOrder = _expeditionOrderRepository.GetById(id);

            ExpeditionOrderWithRouteUrlViewModel ordersViewModel = new ExpeditionOrderWithRouteUrlViewModel();
            ordersViewModel.UserLoged = _userRepository.GetUserLoged();
            ordersViewModel.Order = expeditionOrder;
            ordersViewModel.RouteUrl = routeUrl;

            if (expeditionOrder.PackingListTransportationId != null)
            {
                ordersViewModel.PackingListTransportation = _packingListTransportationRepository.GetById(expeditionOrder.PackingListTransportationId ?? 0);
            }

            var packing = _packingRepository.GetByExpeditionOrderId(id);
            if(packing != null)
            {
                ordersViewModel.PackingInfo = new OrderPackingInfo
                {
                    PackingId = packing.Id,
                    Photo = packing.ImagePath,
                    Status = packing.Status,
                    Observation = packing.Observation
                };
            }

            return View(ordersViewModel);
        }

        [PageForClient]
        public async Task<IActionResult> Create(int? id = null)
        {
            Client client = _clientRepository.GetByUserLoged();
            User userLoged = _userRepository.GetUserLoged();
            

            ExpeditionOrderViewModel model = new ExpeditionOrderViewModel()
            {
                ClientName = client.SocialReason,
                Cnpj = client.Cnpj,
                ClientId = client.Id,
                UserLoged = userLoged

            };

            if (id == null)
            {
                return View(model);
            }
            else
            {
                var order = (await _expeditionOrderRepository.GetAllById(new List<int>() { Convert.ToInt32(id) })).FirstOrDefault();

                if (order != null)
                {
                    model.Id = order.Id;
                    model.ExternalNumber = order.ExternalNumber;
                    model.OrderOrigin = order.OrderOrigin;
                    model.InvoiceAccessKey = order.InvoiceAccessKey;
                    model.InvoiceNumber = order.InvoiceNumber;
                    model.InvoiceSerie = order.InvoiceSerie;
                    model.IssueDate = order.IssueDate;
                    model.DeliveryDate = order.DeliveryDate;
                    model.IssueDate = order.IssueDate;
                    model.Status = (ExpeditionOrderStatus)order.Status;
                    model.ShippingCompany = order.ShippingCompany;
                    model.Obs = order.Obs;
                    model.ClientId = order.ClientId;
                    model.ExpeditionOrderItems = order.ExpeditionOrderItems;
                    model.Name = order.ShippingDetails.Name;
                    model.FantasyName = order.ShippingDetails.FantasyName;
                    model.CpfCnpj = order.ShippingDetails.CpfCnpj;
                    model.Rg = order.ShippingDetails.Rg;
                    model.Address = order.ShippingDetails.Address;
                    model.Number = order.ShippingDetails.Number;
                    model.Complement = order.ShippingDetails.Complement;
                    model.Neighborhood = order.ShippingDetails.Neighborhood;
                    model.Cep = order.ShippingDetails.Cep;
                    model.City = order.ShippingDetails.City;
                    model.Uf = order.ShippingDetails.Uf;
                    model.Phone = order.ShippingDetails.Phone;
                    model.ShippingDetailsId = order.ShippingDetailsId;
                    model.InvoiceValue = order.InvoiceValue.ToString();
                    model.UserLoged = userLoged;
                }

                return View(model);
            }

        }

        [PageForAdmin]
        [HttpGet]
        public async Task<IActionResult> GenerateSimplifiedDanfeList(string[] selecteds)
        {
            try
            {
                var binaryData = await _expeditionOrderService.GenerateSimplifiedDanfe(selecteds.Select(e => Convert.ToInt32(e)).ToList());
                return File(binaryData, "application/pdf;");
            }
            catch (Exception e)
            {
                Log.Error(string.Format("Erro na geração da etiqueta. {0} - {1}", e.Message, e.StackTrace));
                TempData["ErrorMessage"] = e.Message;
            }

            return View("Error", new { RequestId = 0 });
        }

        [PageForAdmin]
        public async Task<IActionResult> CreateAdmin(int? id = null)
        {
            ExpeditionOrderViewModel model = new ExpeditionOrderViewModel();
            User userLoged = _userRepository.GetUserLoged();
            model.UserLoged = userLoged;


            if (id == null)
            {
                return View(model);
            }
            else
            {
                var order = (await _expeditionOrderRepository.GetAllById(new List<int>() { Convert.ToInt32(id) })).FirstOrDefault();

                if (order != null)
                {
                    model.Id = order.Id;
                    model.ClientName = order.ClientName;
                    model.Cnpj = order.Cnpj;
                    model.ExternalNumber = order.ExternalNumber;
                    model.OrderOrigin = order.OrderOrigin;
                    model.InvoiceAccessKey = order.InvoiceAccessKey;
                    model.IssueDate = order.IssueDate;
                    model.DeliveryDate = order.DeliveryDate;
                    model.IssueDate = order.IssueDate;
                    model.Status = (ExpeditionOrderStatus)order.Status;
                    model.ShippingCompany = order.ShippingCompany;
                    model.Obs = order.Obs;
                    model.ClientId = order.ClientId;
                    model.ExpeditionOrderItems = order.ExpeditionOrderItems;
                    model.Name = order.ShippingDetails.Name;
                    model.FantasyName = order.ShippingDetails.FantasyName;
                    model.CpfCnpj = order.ShippingDetails.CpfCnpj;
                    model.Rg = order.ShippingDetails.Rg;
                    model.Address = order.ShippingDetails.Address;
                    model.Number = order.ShippingDetails.Number;
                    model.Complement = order.ShippingDetails.Complement;
                    model.Neighborhood = order.ShippingDetails.Neighborhood;
                    model.Cep = order.ShippingDetails.Cep;
                    model.City = order.ShippingDetails.City;
                    model.Uf = order.ShippingDetails.Uf;
                    model.Phone = order.ShippingDetails.Phone;
                    model.InvoiceNumber = order.InvoiceNumber;
                    model.InvoiceValue = order.InvoiceValue.ToString();
                    model.InvoiceSerie = order.InvoiceSerie;
                    model.ShippingDetailsId = order.ShippingDetailsId;
                    model.UserLoged = userLoged;
                }

                return View(model);
            }
        }

        [PageForAdmin]
        [HttpGet]
        public async Task<List<List<string>>> CheckIfOrderIsMelhorEnvioOrNot(string[] selecteds)
        {
            try
            {
                return await _expeditionOrderService.CheckIfOrderIsMelhorEnvio(selecteds.Select(e => Convert.ToInt32(e)).ToList());
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return null;
            }


        }

        [HttpPost]
        public IActionResult Create(ExpeditionOrderViewModel model, string? products)
        {
            try
            {
                ShippingDetails details = new ShippingDetails()
                {
                    Name = model.Name,
                    FantasyName = model.FantasyName,
                    CpfCnpj = model.CpfCnpj,
                    Rg = model.Rg,
                    Address = model.Address,
                    Number = model.Number,
                    Complement = model.Complement,
                    Neighborhood = model.Neighborhood,
                    Cep = model.Cep,
                    City = model.City,
                    Uf = model.Uf,
                    Phone = model.Phone
                };

                // Converter para decimal usando a cultura brasileira
                decimal invoiceValueInDecimal = Decimal.Parse(model.InvoiceValue);

                ExpeditionOrder order = new ExpeditionOrder
                {
                    ClientName = model.ClientName,
                    Status = ExpeditionOrderStatus.ProcessingPendenting,
                    CreateOrigin = ExpeditionOrderCreateOrigin.PAINEL_CLIENTE,
                    Cnpj = model.Cnpj,
                    ExternalNumber = model.ExternalNumber,
                    OrderOrigin = model.OrderOrigin,
                    InvoiceAccessKey = model.InvoiceAccessKey,
                    InvoiceSerie = model.InvoiceSerie,
                    InvoiceNumber = model.InvoiceNumber,
                    IssueDate = DateTimeHelper.GetCurrentDateTime(),
                    DeliveryDate = model.DeliveryDate,
                    ShippingCompany = model.ShippingCompany,
                    ShippingMethod = model.ShippingMethod,
                    Obs = model.Obs,
                    InvoiceValue = invoiceValueInDecimal,
                    ClientId = model.ClientId,
                    ExpeditionOrderItems = model.ExpeditionOrderItems
                };

                if (model.OrderOrigin == null)
                {
                    order.OrderOrigin = OrderOrigin.ClientPanel;
                    model.OrderOrigin = OrderOrigin.ClientPanel;

                    order.InvoiceAccessKey = "manual";
                }

                if (model.ShippingDetailsId != null && model.ShippingDetailsId > 0)
                {
                    order.ShippingDetailsId = model.ShippingDetailsId;
                    details.Id = Convert.ToInt32(model.ShippingDetailsId);
                    details = _shippingDetailsRepository.Update(details);
                }
                else
                {
                    order.ShippingDetails = details;
                }

                User userLoged = _userRepository.GetUserLoged();

                if (model.Id != null && model.Id > 0)
                {
                    order.Id = Convert.ToInt32(model.Id);
                    order = _expeditionOrderRepository.Update(order);
                }
                else if (model.OrderOrigin == OrderOrigin.ClientPanel)
                {
                     order = _expeditionOrderRepository.AddOrder(order);
                    _expeditionOrderHistoryService.Add(order.Id, "Pedido Gerado manualmente", ExpeditionOrderStatus.ProcessingPendenting, userLoged.Id);
                }
                else if (model.OrderOrigin == OrderOrigin.XMLCreation)
                {
                     order = _expeditionOrderRepository.AddOrder(order);
                    _expeditionOrderHistoryService.Add(order.Id, "Pedido Gerado por XML", ExpeditionOrderStatus.ProcessingPendenting, userLoged.Id);
                }

                // Verify if has tag
                if (model.File != null && model.File.Length > 0)
                {

                    // save new file
                    var (url, format) = _expeditionOrderService.SaveTagUploaded(model.File, order);

                    // Verify if order has a previous tag created
                    var tagSaved = _expeditionOrderTagShippingRepository.GetShippingByExpeditionOrderId(order.Id);

                    // if has, delete and update to the new tag
                    if (tagSaved != null)
                    {
                        if (tagSaved.FileFormat != FileFormatEnum.Url & tagSaved.Url != url)
                        {
                            if (System.IO.File.Exists(tagSaved.Url))
                            {
                                System.IO.File.Delete(tagSaved.Url);
                            }
                        }

                        tagSaved.FileFormat = format;
                        tagSaved.InvoiceAccessKey = order.InvoiceAccessKey;
                        tagSaved.OrderTagOrigin = ShippingMethodEnum.Uploaded;
                        tagSaved.ShippingCode = "";
                        tagSaved.Url = url;
                        tagSaved.EntryDate = DateTime.Now;

                        _expeditionOrderTagShippingRepository.Update(tagSaved);
                    }
                    else
                    {
                        // if doesn't have, create new tag
                        var tag = new ExpeditionOrderTagShipping()
                        {
                            ExpeditionOrderId = order.Id,
                            EntryDate = DateTime.Now,
                            FileFormat = format,
                            InvoiceAccessKey = order.InvoiceAccessKey,
                            OrderTagOrigin = ShippingMethodEnum.Uploaded,
                            ShippingCode = "",
                            Url = url
                        };

                        _expeditionOrderTagShippingRepository.Add(tag);
                    }
                }

                if (products != null)
                {
                    List<ProductValidateQuantityRequestViewModel> productList = JsonConvert.DeserializeObject<List<ProductValidateQuantityRequestViewModel>>(products);
                    foreach (var product in productList)
                    {
                        //ExpeditionOrder By Id
                        ExpeditionOrder expeditionOrderToAddProduct = _expeditionOrderRepository.GetById(order.Id);

                        //Verify if client exist
                        Client client = _clientRepository.GetById(order.ClientId);
                        if (client == null) 
                        { 
                            TempData["ErrorMessage"] = "O cliente e/ou pedido informado não foi encontrado, por favor, digite um válido!"; return View(expeditionOrderToAddProduct); 
                        }

                        //Verify if product exist
                        Product productFinded = _productRepository.GetById(product.ProductId);
                        if (product == null) 
                        {
                            TempData["ErrorMessage"] = "O produto informado não foi encontrado, por favor, digite um produto válido."; 
                            return View(expeditionOrderToAddProduct); 
                        }

                        ExpeditionOrderItemViewModel itemViewmodel = new ExpeditionOrderItemViewModel()
                        {
                            Name = productFinded.Description,
                            Quantity = product.Quantity,
                            Description = productFinded.Description,
                            Ean = productFinded.Ean,
                            ProductId = productFinded.Id,
                            ExpeditionOrderId = order.Id,
                            ClientId = client.Id
                        };

                        ExpeditionOrder orderReturnedAfterAddProduct = _expeditionOrderItemsRepository.AddItem(itemViewmodel);
                    }

                }

                TempData["SuccessMessage"] = "Seu pedido foi criado com sucesso!";

                if(userLoged.PermissionLevel != PermissionLevel.Admin)
                {
                    return RedirectToAction("Orders", "ExpeditionOrder");
                }

                return RedirectToAction("IndexManualOrders", "ExpeditionOrder");

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Não foi possível criar o seu pedido, por favor, tente novamente! Erro: {ex.Message}";

                return RedirectToAction("Orders", "ExpeditionOrder");
            }
        }

        public async Task<IActionResult> ActionApproveOrRecuse([FromQuery] string[] selecteds, [FromQuery] string action, string subject, string? message)
        {
           
            List<MessageAndOrderIdViewModel> errorMessages = new List<MessageAndOrderIdViewModel>();
            
            foreach (var selected in selecteds)
            {
                int currentProcessing = 0;
                try
                {
                    int id = Convert.ToInt32(selected);
                    currentProcessing = id;

                    switch (action)
                    {
                        case "approve":
                            await _expeditionOrderService.Approve(id);
                            break;
                        case "recuse":
                            await _expeditionOrderService.UpdateOrder(id, ExpeditionOrderStatus.ProcessingRefused);
                            break;
                    }
                    
                }
                catch (ProblemWithAcceptOrderException e)
                {
                    errorMessages.Add(new MessageAndOrderIdViewModel()
                    {
                        Message = e.Message,
                        OrderId = e.OrderId
                    });

                }
                catch (Exception ex)
                {
                    errorMessages.Add(new MessageAndOrderIdViewModel()
                    {
                        Message = "Ocorreu um erro inesperado ao processar o Pedido: " + ex.Message,
                        OrderId = currentProcessing
                    });
                }
            }

            if (errorMessages.Count > 0) return BadRequest(errorMessages);

            return NoContent();
        }

        public async Task<IActionResult> Cancel(int id, string subject, string? message)
        {
            try
            {
                await _expeditionOrderService.Cancel(id, subject, message);
                TempData["SuccessMessage"] = "Pedido cancelado com sucesso!";

                ExpeditionOrderFilterViewModel request = new ExpeditionOrderFilterViewModel();
                request.PageNumber = 1;

                ExpeditionOrderIndexViewModel reponseVwa = new ExpeditionOrderIndexViewModel();

                return await RenderOrders(ExpeditionOrdersPage.Orders, reponseVwa, request);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        public async Task<IActionResult> ShowTag(int id)
        {
            try
            {
                ExpeditionOrderTagShipping tag = await _expeditionOrderTagShippingRepository.GetShippingByExpeditionOrderIdAsync(id);

                if (tag == null)
                {
                    throw new Exception("Etiqueta não encontrada");
                }

                if (tag.OrderTagOrigin == ShippingMethodEnum.MelhorEnvio)
                {
                    return Redirect(tag.Url);
                }
                else
                {
                    byte[] binaryData = await _expeditionOrderService.GetProcessedTag(tag);
                    return File(binaryData, "application/pdf;");
                }   
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return await Orders();
        }


        public async Task<IActionResult> ProcessTag(int id, string page)
        {
            try
            {
                await _expeditionOrderService.ProcessTag(id);
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            if (page == "Integração")
                return await Index();

            return await Orders();
        }

        public IActionResult SetProduct(int id)
        {
            ExpeditionOrder expeditionOrder = _expeditionOrderRepository.GetById(id);

            return View(expeditionOrder);
        }


        [HttpPost]
        public IActionResult SetProduct(ExpeditionOrderItemViewModel item)
        {
            try
            {
                //ExpeditionOrder By Id
                ExpeditionOrder expeditionOrder = _expeditionOrderRepository.GetById(item.Id);

                if(item.ExpeditionOrderId == 0)
                {
                    item.ExpeditionOrderId = expeditionOrder.Id;
                }

                //Verify if client exist
                Client client = _clientRepository.GetById(item.ClientId);
                if (client == null)
                {
                    TempData["ErrorMessage"] = "O cliente e/ou pedido informado não foi encontrado, por favor, digite um válido!";
                    return RedirectToAction("SetProduct", new { id = expeditionOrder.Id });
                }

                //Verify if product exist
                Product product = _productRepository.GetByEan(item.Ean, item.ClientId);
                if (product == null) 
                { 
                    TempData["ErrorMessage"] = "O produto informado não foi encontrado, por favor, digite um produto válido.";
                    return RedirectToAction("SetProduct", new { id = expeditionOrder.Id });
                }

                if(expeditionOrder.ExpeditionOrderItems.Any(x => x.Ean == item.Ean))
                {
                    TempData["ErrorMessage"] = "Não é possível adicionar duas vezes o mesmo produto no pedido.";
                    return RedirectToAction("SetProduct", new { id = expeditionOrder.Id });
                }

                var quantity = product.StockQuantity;

                double itemQuantity = (double)item.Quantity;

                if (itemQuantity > quantity)
                {
                    TempData["ErrorMessage"] = $"Ops! Não foi possível inserir esse produto, {product.Description} possui apenas {quantity} em nosso estoque";
                    return RedirectToAction("SetProduct", new { id = expeditionOrder.Id });
                }
                else
                {
                    ExpeditionOrder order = _expeditionOrderItemsRepository.AddItem(item);
                    TempData["SuccessMessage"] = "Os itens foram adicionados com sucesso!";
                    return RedirectToAction("SetProduct", new {id = order.Id});
                }

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Não foi possível adicionar os itens, por favor, tente novamente!";
                return View(item);
            }
        }


        [HttpPost]
        public async Task<IActionResult> GeneratePickingList(string[] selecteds, string observation, int priority, int marketplace)
        {
            ExpeditionOrderIndexViewModel view = new ExpeditionOrderIndexViewModel();

            List<ExpeditionOrder> orders = await _expeditionOrderRepository.GetAllById(selecteds.Select(e => Convert.ToInt32(e)));

            User userLoged = _userRepository.GetUserLoged();

            try
            {
                var hasPickingList = orders.Where(e => e.PickingListId > 0).ToList();

                // There are some Order that wasn't separated
                if (hasPickingList.Count > 0)
                {
                    var message = string.Join(", ", hasPickingList.Select(e => e.ExternalNumber).ToList());

                    TempData["ErrorMessage"] = $"Não possível gerar o Picking List dos pedidos. Os pedidos {message} já foram para separação";
                }
                else
                {
                    PriorityEnum priorityEnum = (PriorityEnum)Enum.ToObject(typeof(PriorityEnum), priority);
                    MarketPlaceEnum marketplaceEnum = (MarketPlaceEnum)Enum.ToObject(typeof(MarketPlaceEnum), marketplace);
                    PickingList picking = await _expeditionOrderService.GeneratePickingList(orders, observation, priorityEnum, marketplaceEnum);

                    await _pickingListHistoryService.Add(picking.Id, observation, PickingListStatus.Gerado, userLoged.Id);

                    foreach (var order in orders)
                    {
                        await _expeditionOrderHistoryService.Add(order.Id, "Separação Iniciada", ExpeditionOrderStatus.InPickingList, userLoged.Id);
                    }
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return await Orders();
        }

        public async Task<IActionResult> Orders()
        {
            ExpeditionOrderIndexViewModel reponseVwa = new ExpeditionOrderIndexViewModel();
            return await RenderOrders(ExpeditionOrdersPage.Orders, reponseVwa, new ExpeditionOrderFilterViewModel
            {
                PageNumber = 1,

            });
        }


        private async Task<IActionResult> RenderOrders(ExpeditionOrdersPage page, ExpeditionOrderIndexViewModel? response, ExpeditionOrderFilterViewModel? request)
        {
            if (request == null)
                request = new ExpeditionOrderFilterViewModel
                {
                    PageNumber = 1
                };

            if (page == ExpeditionOrdersPage.Integration && response.Orders == null)
            {
                PaginationBase<ExpeditionOrderWithPickingListViewModel> pag = await _expeditionOrderService.GetAllByUserAndStatus(response.IntegrationStatus, request);
                response.Orders = pag;
            }
            else if (page == ExpeditionOrdersPage.Orders && response.Orders == null)
            {
                PaginationBase<ExpeditionOrderWithPickingListViewModel> pag = await _expeditionOrderService.GetAllByUserAndStatus(response.OrdersStatus, request);
                response.Orders = pag;
            }

            response.Filter = new ExpeditionOrderFilterViewModel();
            response.Filter = request;
            response.Page = page;
            response.UserLoged = _userRepository.GetUserLoged();
            response.TotalPages = response.Orders.TotalPages;
            response.PageNumber = response.Orders.PageNumber;

            return View("Index", response);
        }

        public async Task<IActionResult> ExpeditionOrderFilter(ExpeditionOrderFilterViewModel orderVm)
        {
            try
            {
                ExpeditionOrderIndexViewModel view = new ExpeditionOrderIndexViewModel();

                orderVm.Client = _clientRepository.GetByUserLoged();

                var statusList = new List<ExpeditionOrderStatus>();

                if (orderVm.PageFilter == ExpeditionOrdersPage.Integration)
                {
                    statusList = view.IntegrationStatus;
                }
                else if (orderVm.PageFilter == ExpeditionOrdersPage.Orders)
                {
                    statusList = view.OrdersStatus;
                }

                PaginationBase<ExpeditionOrderWithPickingListViewModel> vm = await _expeditionOrderService.GetByFilter(orderVm, statusList);

                view.Orders = vm;
                orderVm.TotalPages = vm.TotalPages;

                if (vm.Data.Count() == 0)
                    TempData["ErrorMessage"] = "Ops, não achamos nenhum pedido com o(s) filtro(s) informado(s), por favor, tente novamente!";

                return await RenderOrders(orderVm.PageFilter, view, orderVm);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Não foi possível filtrar agora, por favor, tente novamente mais tarde.";
                throw new Exception("Error: " + ex);

            }
        }

        public IActionResult UploadXmlPage()
        {
            return View("UploadFiles");
        }


        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            List<string> errors = new List<string>();
            List<string> results = new List<string>();
            ExpeditionOrder order = null;

            if (files.Count == 0)
            {
                errors.Add("Erro: Arquivo(s) não selecionado(s)");
                ViewData["Error"] = errors;
                ViewData["Result"] = results;
                return View("UploadFiles", ViewData);
            }

            foreach (var file in files)
            {
                //verifica se existem arquivos 
                if (file == null || file.Length == 0)
                {
                    errors.Add($"Erro: Corpo do arquivo não encontrado");
                    continue;
                }


                if (!file.FileName.Contains(".xml"))
                {
                    errors.Add($"Erro: O arquivo {file.FileName} não é um XML, selecione um arquivo válido.");
                    continue;
                }

                Stream fileStream = file.OpenReadStream();

                using var sr = new StreamReader(fileStream, Encoding.UTF8);

                var content = sr.ReadToEnd();

                //NFE
                var result = await _nfeService.ImportNfeAsync(content, 0, 0, true);

                User user = _userRepository.GetUserLoged();


                if (result.Success)
                {
                    order = await _expeditionOrderService.CreateOrderByNfe(result);

                    TempData["SuccessMessage"] = "Seu pedido foi criado com sucesso! Agora faça a revisão e edite se precisar.";

                    switch (user.PermissionLevel)
                    {
                        case PermissionLevel.Client:
                            return RedirectToAction("Create", "ExpeditionOrder", new { id = order.Id });

                        case PermissionLevel.Admin:
                            return RedirectToAction("CreateAdmin", "ExpeditionOrder", new { id = order.Id });

                    }

                }
                else
                {
                    errors.Add($"Erro: Não foi possível importar o arquivo {file.FileName}. {result.Message}");
                }
            }

            Client client = _clientRepository.GetByUserLoged();

            if (client != null)
            {
                if (errors.Count > 0)
                {
                    TempData["ErrorMessage"] = errors[0];
                }
                else if (results.Count > 0)
                {
                    TempData["SuccessMessage"] = results[0];
                }

                return RedirectToAction("CreateByXML");
            }
            else
            {
                if (errors.Count > 0)
                {
                    TempData["ErrorMessage"] = errors[0];
                }
                else if (results.Count > 0)
                {
                    TempData["SuccessMessage"] = results[0];
                }

                ExpeditionOrderViewModel model = new ExpeditionOrderViewModel();

                return View("CreateAdmin", model);
            }


        }



        public async Task<IActionResult> ListExpeditionOrdersInGroup(int clientId)
        {
            List<ExpeditionOrder> expeditionOrders = await _expeditionOrderRepository.GetOrdersIsNotGeneratedReturnInvoiceByClientId(clientId);

            ExpeditionOrderWithRouteUrlViewModel orderVm = new ExpeditionOrderWithRouteUrlViewModel()
            {
                ExpeditionOrders = expeditionOrders,
                Ids = string.Join(",", expeditionOrders.Select(e => e.Id))
            };

            return View(orderVm);
        }

        [HttpGet]
        public async Task<List<ExpeditionOrderHistory>> GetExpeditionOrderHistoryById(int orderId)
        {
            try
            {
                List<ExpeditionOrderHistory> list = await _expeditionOrderHistoryService.GetByOrderId(orderId);

                return list;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"ERRO: {ex}";
                List<ExpeditionOrderHistory> list = new List<ExpeditionOrderHistory>();
                return list;
            }

        }

        public async Task<IActionResult> FinalizeSetProducts(OrderOrigin? origin, int orderId)
        {
            ExpeditionOrder order = await _expeditionOrderRepository.GetOrderByIdAsync(orderId);

            if (!order.ExpeditionOrderItems.Any())
            {
                TempData["ErrorMessage"] = $"Ops! Não é possível criar um pedido sem produto. Por favor, anexe algum produto ao pedido para prosseguir.";
                return RedirectToAction("SetProduct", "ExpeditionOrder", new { id = order.Id });
            }

            if (origin == OrderOrigin.ClientPanel)
            {
                TempData["SuccessMessage"] = $"Pedido manual criado com sucesso!";
            }
            else
            {
                TempData["SuccessMessage"] = $"Pedido por XML criado com sucesso!";
            }

            ExpeditionOrderFilterViewModel request = new ExpeditionOrderFilterViewModel();
            request.PageNumber = 1;

            ExpeditionOrderIndexViewModel reponseVwa = new ExpeditionOrderIndexViewModel();

            return await RenderOrders(ExpeditionOrdersPage.Orders, reponseVwa, request);

        }




        [HttpPost]
        public async Task<IActionResult> UpdateOrderVolume(int id, int volume)
        {

            if (id <= 0 || volume <= 0)
            {
                TempData["ErrorMessage"] = "Não é possível selecionar 0 volumes para um Empacotamento";
                return RedirectToAction("Index", "Packing");
            }

           Packing packing =  await _packingRepository.GetByIdAsync(id);

            ExpeditionOrder order = packing.ExpeditionOrder;

            order.VolumeQuantity = volume;

            _expeditionOrderRepository.Update(order);


            TempData["SuccessMessage"] = "Volume do empacotamento atualizado com sucesso!";

            return RedirectToAction("Index", "Packing");
        }
       

        public async Task<IActionResult> DeletePackingListOnOrder(int orderId, int packingListId)
        {

            ExpeditionOrder order = await _expeditionOrderRepository.GetOrderByIdAsync(orderId);

            if (order.PackingListTransportationId != null)
            {
                order.PackingListTransportationId = null;
            }
            else
            {
                TempData["ErrorMessage"] = "Esse pedido não está em nenhum romaneio";
            }

            _expeditionOrderRepository.Update(order);

            TempData["SuccessMessage"] = $"Pedido removido do romaneio com sucesso!";

            return RedirectToAction("Details", "PackingListTransportations", new { id = packingListId });
        }

        public async Task<IActionResult> ActionApproveOrRecuseForManualOrders([FromQuery] string[] selecteds, [FromQuery] string action, string subject, string? message)
        {
            List<MessageAndOrderIdViewModel> errorMessages = new List<MessageAndOrderIdViewModel>();

            User userLoged = _userRepository.GetUserLoged();

            foreach (var selected in selecteds)
            {
                try
                {
                    int id = Convert.ToInt32(selected);

                    switch (action)
                    {
                        case "approve":
                            await _expeditionOrderService.Approve(id);
                            break;
                        case "recuse":
                            await _expeditionOrderService.UpdateOrder(id, ExpeditionOrderStatus.ProcessingRefused);
                            break;
                    }
                }
                catch (ProblemWithAcceptOrderException e)
                {
                    errorMessages.Add(new MessageAndOrderIdViewModel()
                    {
                        Message = e.Message,
                        OrderId = e.OrderId
                    });

                }
                catch (Exception ex)
                {
                    errorMessages.Add(new MessageAndOrderIdViewModel()
                    {
                        Message = "Ocorreu um erro inesperado ao processar o Pedido: " + ex.Message,
                        OrderId = Convert.ToInt32(selected)
                    }) ;
                }
            }

            if (errorMessages.Count > 0) return BadRequest(errorMessages);

            return NoContent();
        }


        public async Task<IActionResult> DeleteExpeditionOrdemItem(int orderId, int orderItem)
        {

            var result = await _expeditionOrderItemsRepository.DeleteByItemId(orderItem);

            if (result)
            {
                TempData["SuccessMessage"] = $"Produto removido do pedido com sucesso!";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível remover esse produto!";
            }

            return RedirectToAction("SetProduct", "ExpeditionOrder", new { id = orderId });
        }

        [HttpPost]
        public async Task<bool> ValidateIfNoteAlreadyExists(int id, int invoiceNumber, int clientId)
        {
            bool orderExists = await _expeditionOrderService.CheckIfOrderExistsByInvoiceNumberAndClientId(id, invoiceNumber, clientId);

            return orderExists;
        }


    }
}
