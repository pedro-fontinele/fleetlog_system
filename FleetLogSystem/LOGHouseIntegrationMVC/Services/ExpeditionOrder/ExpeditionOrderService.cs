using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Models;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.ViewModels;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Controllers;
using LOGHouseSystem.Services.Helper;
using System.Text;
using System.Drawing;
using LOGHouseSystem.Infra.Helpers.CustomExceptions;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension;
using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension.Dto.Response;
using LOGHouseSystem.Models.SendEmails;
using LOGHouseSystem.Adapters.API.MercadoLivre.Response;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace LOGHouseSystem.Services
{
    public class ExpeditionOrderService : IExpeditionOrderService
    {
        private AppDbContext _context;
        
        
        private IExpeditionOrderRepository _expeditionOrderRepository;
        private IPickingListRepository _pickingListRepository;
        private IPickingListItemRepository _pickingListItemRepository;
        private IClientsRepository _clientsRepository;
        private IPositionAndProductRepository _positionAndProductRepository;
        private IInvoiceRepository _invoiceRepository;
        private IZplToPdfService _zplToPdfService;
        protected SessionHelper _session = new SessionHelper();
        private ProductService _productService = new ProductService();
        private IProductRepository _productRepository;
        private IPackingRepository _packingRepository;
        private IPackingItemRepository _packingItemRepository;
        private readonly IExpeditionOrderItemsRepository _expeditionOrderItemsRepository;
        private readonly IEmailService _emailService;
        private readonly IExpeditionOrderHistoryService _expeditionOrderHistoryService;
        private readonly IInvoiceService _invoiceService;
        private readonly ISimplifiedDanfeService _simplifiedDanfeService;
        private readonly IExpeditionOrderTagShippingRepository _expeditionOrderTagShippingRepository;
        private readonly IMelhorEnvioAPIServices _melhorEnvioAPIServices;
        private readonly ITagService _tagService;
        private readonly IUserRepository _userRepository;
        private readonly IExpeditionOrdersLotNotFoundedRepository _expeditionOrdersLotNotFoundedRepository;

        public ExpeditionOrderService(            
            IExpeditionOrderRepository expeditionOrderRepository,
            IPickingListRepository pickingListRepository,
            IPickingListItemRepository pickingListItemRepository,
            IClientsRepository clientsRepository,
            IPositionAndProductRepository positionAndProductRepository,
            IInvoiceRepository invoiceRepository,
            IZplToPdfService zplToPdfService,
            IProductRepository productRepository,
            IPackingRepository packingRepository,
            AppDbContext context,
            IPackingItemRepository packingItemRepository,
            IExpeditionOrderItemsRepository expeditionOrderItemsRepository,
            IEmailService emailService,
            IExpeditionOrderHistoryService expeditionOrderHistoryService,
            IInvoiceService invoiceService,
            ISimplifiedDanfeService simplifiedDanfeService,
            IExpeditionOrderTagShippingRepository expeditionOrderTagShippingRepository,
            IMelhorEnvioAPIServices melhorEnvioAPIServices,
            IUserRepository userRepository,
            ITagService tagService,
            IExpeditionOrdersLotNotFoundedRepository expeditionOrdersLotNotFoundedRepository)
        {
            _context = context;            
            _expeditionOrderRepository = expeditionOrderRepository;
            _pickingListRepository = pickingListRepository;
            _pickingListItemRepository = pickingListItemRepository;
            _clientsRepository = clientsRepository;
            _positionAndProductRepository = positionAndProductRepository;
            _invoiceRepository = invoiceRepository;
            _zplToPdfService = zplToPdfService;
            _productRepository = productRepository;
            _packingRepository = packingRepository;
            _packingItemRepository = packingItemRepository;
            _expeditionOrderItemsRepository = expeditionOrderItemsRepository;
            _emailService = emailService;
            _expeditionOrderHistoryService = expeditionOrderHistoryService;
            _invoiceService = invoiceService;
            _simplifiedDanfeService = simplifiedDanfeService;
            _expeditionOrderTagShippingRepository = expeditionOrderTagShippingRepository;
            _melhorEnvioAPIServices = melhorEnvioAPIServices;
            _tagService = tagService;
            _userRepository = userRepository;
            _expeditionOrdersLotNotFoundedRepository = expeditionOrdersLotNotFoundedRepository;
        }

        public async Task<PickingList> GeneratePickingList(List<ExpeditionOrder> orders, string obs, PriorityEnum priority, MarketPlaceEnum marketplace)
        {
            // Open transaction 
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    PickingList pickingList = new PickingList()
                    {
                        Status = PickingListStatus.Gerado,
                        Description = obs,
                        Quantity = orders.Select(e => e.ExpeditionOrderItems != null ? e.ExpeditionOrderItems.Select(x => x.Quantity).Sum() : 0).Sum(),
                        Responsible = "",
                        Priority = priority,
                        MarketPlace = marketplace
                    };

                    pickingList = await _pickingListRepository.Create(pickingList);

                    foreach (var item in orders)
                    {

                        foreach (var itemList in item.ExpeditionOrderItems)
                        {
                            var pickingListItemCreated = await _pickingListItemRepository.GetByProductIdAsync(itemList.ProductId, pickingList.Id);

                            if (pickingListItemCreated != null)
                            {
                                pickingListItemCreated.Quantity += Convert.ToInt32(itemList.Quantity);
                                _pickingListItemRepository.Update(pickingListItemCreated);
                            }
                            else
                            {
                                var address = await _positionAndProductRepository.GetFirstProductAddressAsync(Convert.ToInt32(itemList.ProductId));

                                PickingListItem pickingListItem = new PickingListItem()
                                {

                                    PickingListId = pickingList.Id,
                                    ProductId = Convert.ToInt32(itemList.ProductId),
                                    Quantity = Convert.ToInt32(itemList.Quantity),
                                    Address = address?.AddressingPosition?.Name == null ? "" : address?.AddressingPosition?.Name,
                                    ItemStatus = PickingListItemStatus.Gerado
                                };

                                pickingListItem = await _pickingListItemRepository.Create(pickingListItem);
                            }
                        }

                        item.Status = ExpeditionOrderStatus.InPickingList;
                        item.PickingListId = pickingList.Id;

                        _expeditionOrderRepository.Update(item);
                    }

                    await transaction.CommitAsync();
                    return pickingList;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }

            }
        }

        public async Task<PaginationBase<ExpeditionOrderWithPickingListViewModel>> GetAllByUserAndStatus(List<ExpeditionOrderStatus> status, ExpeditionOrderFilterViewModel request)
        {
            User userLoged = _session.SearchUserSession();

            if (userLoged == null || userLoged.Id <= 0)
                throw new Exception("Erro ao buscar usuário");


            Client client = _clientsRepository.GetByUserId(userLoged.Id);
            PaginationBase<ExpeditionOrderWithPickingListViewModel> pagination = new();


            if (userLoged.PermissionLevel == PermissionLevel.Admin)
            {
                PaginationBase<ExpeditionOrder> paginationOrder = await _expeditionOrderRepository.GetAllOrdersByStatusAsync(status, request);

                pagination = await ConvertOrderExpeditionToViewModel(paginationOrder);


                return pagination;
            }

            PaginationBase<ExpeditionOrder> orders = await _expeditionOrderRepository.GetOrdersByClientIdAndbyStatusAsync(client.Id, status, request);

            pagination = await ConvertOrderExpeditionToViewModel(orders);

            return pagination;
        }

        public async Task ProcessTag(int id)
        {
            var order = await _expeditionOrderRepository.GetOrderByIdAsync(id);

            await ProcessTag(order);
        }

        public async Task ProcessTag(ExpeditionOrder order)
        {
            try
            {
                await TryProcessTag(order);
            }
            catch (Exception ex)
            {
                await CatchProcessTag(order, ex);
                throw;
            }
            
        }

        private async Task TryProcessTag(ExpeditionOrder order)
        {
            await _tagService.ProcessMarketplaceTag(order);
            await UnblockShippingTag(order);
        }

        private async Task CatchProcessTag(ExpeditionOrder order, Exception ex)
        {
            Log.Error(string.Format("Houve um erro no momento de processar a etiqueta. {0} - {1} - {2}", ex.Message, ex.InnerException, ex.StackTrace));
            await BlockShippingTag(order);            
        }

        private async Task BlockShippingTag(ExpeditionOrder order)
        {
            await ShippingTagBlocked(order, true);
        }

        private async Task UnblockShippingTag(ExpeditionOrder order)
        {
            await ShippingTagBlocked(order, false);
        }

        private async Task ShippingTagBlocked(ExpeditionOrder order, bool shippingBlocked)
        {
            order.ShippingTagBlocked = shippingBlocked;
            await _expeditionOrderRepository.UpdateAsync(order);
        }

        public (string, FileFormatEnum) SaveTagUploaded(IFormFile file, ExpeditionOrder order)
        {

            string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            string[] allowedFormatsImage = { ".jpg", ".jpeg", ".png", ".gif" };

            FileFormatEnum fileFormatFormat;

            var filePath = $"{Environment.TagUploaded}/{order.Id}{fileExtension}";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            if (allowedFormatsImage.Contains(fileExtension))
            {
                fileFormatFormat = FileFormatEnum.Image;

                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);

                    using (var img = Image.FromStream(memoryStream))
                    {
                        img.Save(filePath);
                    }
                }

            }
            else if (fileExtension == ".zpl")
            {
                Stream fileStream = file.OpenReadStream();

                using var sr = new StreamReader(fileStream, Encoding.UTF8);

                var zplFile = sr.ReadToEnd();

                fileFormatFormat = FileFormatEnum.Zpl;

                File.WriteAllText(filePath, zplFile);
            }
            else if (fileExtension == ".pdf")
            {
                fileFormatFormat = FileFormatEnum.Pdf;

                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            else if (fileExtension == ".txt")
            {
                Stream fileStream = file.OpenReadStream();

                using var sr = new StreamReader(fileStream, Encoding.UTF8);

                var zplFile = sr.ReadToEnd();

                fileFormatFormat = FileFormatEnum.Txt;

                File.WriteAllText(filePath, zplFile);
            }
            else
            {
                throw new Exception("O arquivo enviado não é uma imagem válida.");
            }

            return (filePath, fileFormatFormat);
        }

        public async Task<ExpeditionOrder> UpdateOrder(int id, ExpeditionOrderStatus newState, DateTime? finalizeDate = null)
        {
            ExpeditionOrder order = await _expeditionOrderRepository.GetOrderByIdAsync(id);

            if (order.Status == newState)
                throw new Exception("O status do pedido já foi atualizado.");

            order.Status = newState;

            if (finalizeDate != null)
            {
                order.FinalizeDate = finalizeDate;
            }

            order = _expeditionOrderRepository.Update(order);

            return order;
        }


        public async Task<PaginationBase<ExpeditionOrderWithPickingListViewModel>> ConvertOrderExpeditionToViewModel(PaginationBase<ExpeditionOrder> ordersList)
        {
            PaginationBase<ExpeditionOrderWithPickingListViewModel> orderListVM = new();

            foreach (var order in ordersList.Data)
            {

                ExpeditionOrderWithPickingListViewModel orderVM = new ExpeditionOrderWithPickingListViewModel()
                {
                    Id = order.Id,
                    ClientName = order.ClientName,
                    Cnpj = order.Cnpj,
                    ExternalNumber = order.ExternalNumber,
                    OrderOrigin = order.OrderOrigin,
                    InvoiceAccessKey = order.InvoiceAccessKey,
                    IssueDate = order.IssueDate,
                    DeliveryDate = order.DeliveryDate,
                    Status = order.Status,
                    ShippingCompany = order.ShippingCompany,
                    Obs = order.Obs,
                    ShippingMethod = order.ShippingMethod,
                    ShippingMethodCodeOrder = order.ShippingMethodCodeOrder,
                    ClientId = order.ClientId,
                    ExpeditionOrderItems = order.ExpeditionOrderItems,
                    ShippingDetailsId = order.ShippingDetailsId,
                    ShippingDetails = order.ShippingDetails,
                    ExpeditionOrderTagShipping = order.ExpeditionOrderTagShipping,
                    InvoiceNumber = order.InvoiceNumber,
                    ShippingTagBlocked = order.ShippingTagBlocked
                };


                PickingList picking = await _pickingListRepository.GetByOrderExpeditionIdAsync(order.Id);

                orderVM.PickingList = picking;

                orderListVM.Data.Add(orderVM);

            }

            orderListVM.PageSize = ordersList.PageSize;
            orderListVM.PageNumber = ordersList.PageNumber;
            orderListVM.TotalPages = ordersList.TotalPages;
            orderListVM.TotalRegisters = ordersList.TotalRegisters;
            orderListVM.FirstRegisterInActualPage = ordersList.FirstRegisterInActualPage;
            orderListVM.LastRegisterInActualPage = ordersList.LastRegisterInActualPage;


            return orderListVM;
        }

        public async Task<bool> CheckIfOrderExistsByExternalNumber(ExpeditionOrder expeditionOrder, OrderOrigin origin)
        {
            
            ExpeditionOrder orderByExternalNumber = await _expeditionOrderRepository.GetByExternalNumber(expeditionOrder, origin);
            if (orderByExternalNumber != null) return true;

            //check accesskey
            orderByExternalNumber = _expeditionOrderRepository.GetQueryableFilter(new ExpeditionOrderFilterViewModel
            {
                ClientId = expeditionOrder.ClientId,
                InvoiceNumber = expeditionOrder.InvoiceNumber.ToString()
            },null).FirstOrDefault();
            if (orderByExternalNumber != null) return true;

            //var invoice = await _invoiceService.GetByAcessKeyAsync(expeditionOrder.InvoiceAccessKey);
            //if(invoice != null) return true;

            return false;
        }


        public async Task<PaginationBase<ExpeditionOrderWithPickingListViewModel>> GetByFilter(ExpeditionOrderFilterViewModel filter, List<ExpeditionOrderStatus> statusToFilter)
        {

            PaginationBase<ExpeditionOrder> order = await _expeditionOrderRepository.GetByFilter(filter, statusToFilter);


            return await ConvertOrderExpeditionToViewModel(order);

        }

        public async Task<ExpeditionOrder> CreateOrderByNfe(ResponseDTO responseDTO)
        {
            var client = await _clientsRepository.FindByCnpjAsync(responseDTO.NFeProc.NFe.InfNFe.Emit.CNPJ);


            if (client == null)
            {
                throw new Exception("Não foi possivel encontrar o Client através do emitente.");
            }

            var expeditionOrder = await _expeditionOrderRepository.GetOrderByInvoiceAccessKeyAsync(responseDTO.NFeProc.ProtNFe.InfProt.ChNFe.ToString());

            if (expeditionOrder != null)
            {
                return expeditionOrder;
            }

            // Create base model
            var model = new ExpeditionOrder()
            {
                ClientName = responseDTO.NFeProc.NFe.InfNFe.Emit.XNome,
                Cnpj = responseDTO.NFeProc.NFe.InfNFe.Emit.CNPJ,
                ExternalNumber = responseDTO.NFeProc.ProtNFe.InfProt.ChNFe.ToString(),
                InvoiceAccessKey = responseDTO.NFeProc.ProtNFe.InfProt.ChNFe.ToString(),
                InvoiceNumber = Convert.ToInt32(responseDTO.NFeProc.NFe.InfNFe.Ide.NNF),
                InvoiceSerie = Convert.ToInt32(responseDTO.NFeProc.NFe.InfNFe.Ide.Serie),
                OrderOrigin = OrderOrigin.XMLCreation,
                IssueDate = responseDTO.NFeProc.NFe.InfNFe.Ide.DhEmi,
                ShippingCompany = null, //?
                Obs = "",
                Status = ExpeditionOrderStatus.ProcessingPendenting,
                ShippingMethod = ShippingMethodEnum.Other,
                ShippingMethodCodeOrder = null,
                InvoiceValue = responseDTO.NFeProc.NFe.InfNFe.Total.ICMSTot.vNF,
                ClientId = client.Id
            };

            if (model != null)
            {
                string doc = null;

                if (responseDTO.NFeProc.NFe.InfNFe.Dest.CNPJ != null)
                {
                    doc = responseDTO.NFeProc.NFe.InfNFe.Dest.CNPJ.ToString();
                }
                else if (responseDTO.NFeProc.NFe.InfNFe.Dest.CPF != null)
                {
                    doc = responseDTO.NFeProc.NFe.InfNFe.Dest.CPF.ToString();
                }
                else
                {
                    doc = responseDTO.NFeProc.NFe.InfNFe.Dest.IdEstrangeiro.ToString();
                }

                model.ShippingDetails = new ShippingDetails()
                {
                    Name = responseDTO.NFeProc.NFe.InfNFe.Dest.XNome,
                    CpfCnpj = doc,
                    Address = responseDTO.NFeProc.NFe.InfNFe.Dest.Ender.XLgr,
                    Number = responseDTO.NFeProc.NFe.InfNFe.Dest.Ender.Nro,
                    Complement = responseDTO.NFeProc.NFe.InfNFe.Dest.Ender.XCpl,
                    Neighborhood = responseDTO.NFeProc.NFe.InfNFe.Dest.Ender.XBairro,
                    Cep = responseDTO.NFeProc.NFe.InfNFe.Dest.Ender.CEP,
                    City = responseDTO.NFeProc.NFe.InfNFe.Dest.Ender.XMun,
                    Uf = responseDTO.NFeProc.NFe.InfNFe.Dest.Ender.UF,
                };

                model.ShippingDetailsId = model.ShippingDetails.Id;


                List<ExpeditionOrderItem> orderList = new List<ExpeditionOrderItem>();

                foreach (var item in responseDTO.NFeProc.NFe.InfNFe.Det)
                {

                    Product product;
                    if (item.Prod.CEAN.Contains("|"))
                    {
                        var mutipleEan = item.Prod.CEAN.ToString().Split('|');
                        product = await _productRepository.GetByEanAsync(mutipleEan[0], client.Id);

                    }
                    else if (item.Prod.CEAN != "SEM GTIN")
                    {
                        product = await _productRepository.GetByEanAsync(item.Prod.CEAN, client.Id);
                    }
                    else
                    {
                        product = await _productRepository.GetByCodeAsync(item.Prod.CProd, client.Id);
                    }

                    ExpeditionOrderItem orderItem = new ExpeditionOrderItem()
                    {
                        Name = item.Prod.XProd,
                        Quantity = Convert.ToDecimal(item.Prod.QCom),
                        Description = item.Prod.XProd,
                        ExternalNumberItem = item.Prod.CProd,
                        Value = Convert.ToDecimal(item.Prod.VUnCom),
                        Ean = item.Prod.CEAN,
                        ExpeditionOrderId = model.Id,
                        ProductId = product.Id
                    };

                    orderList.Add(orderItem);

                }

                model.ExpeditionOrderItems = orderList;
            }

            ExpeditionOrder expeditionInserted = null;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    expeditionInserted = await _expeditionOrderRepository.Add(model);
                    var invoice = await _invoiceRepository.GetByAcessKeyAsync(model.InvoiceAccessKey);
                    invoice.ExpeditionOrderId = expeditionInserted.Id;

                    await _invoiceRepository.UpdateAsync(invoice);

                    await transaction.CommitAsync();

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return expeditionInserted;
        }

        public async Task<byte[]> GetProcessedTag(ExpeditionOrderTagShipping tag)
        {
            string content = null;

            List<FileConvert> file = new List<FileConvert>();

            switch (tag.FileFormat)
            {
                case FileFormatEnum.Zpl:
                    content = await tag.Url.GetZplFileContentAsync();

                    if (content == null)
                    {
                        throw new Exception("Etiqueta não encontrada");
                    }

                    file.Add(new FileConvert()
                    {
                        Content = content,
                        Format = tag.FileFormat,
                        OrderNumber = Convert.ToInt32(tag.ExpeditionOrderId),
                        Type = FileTypeEnum.Tag
                    });
                    break;
                case FileFormatEnum.Pdf:

                    file.Add(new FileConvert()
                    {
                        Content = tag.Url,
                        Format = tag.FileFormat,
                        OrderNumber = Convert.ToInt32(tag.ExpeditionOrderId),
                        Type = FileTypeEnum.Tag
                    });
                    break;
                case FileFormatEnum.Image:

                    file.Add(new FileConvert()
                    {
                        Content = tag.Url,
                        Format = tag.FileFormat,
                        OrderNumber = Convert.ToInt32(tag.ExpeditionOrderId),
                        Type = FileTypeEnum.Tag
                    });
                    break;
                case FileFormatEnum.Txt:
                    content = await tag.Url.GetZplFileContentAsync();

                    file.Add(new FileConvert()
                    {
                        Content = content,
                        Format = tag.FileFormat,
                        OrderNumber = Convert.ToInt32(tag.ExpeditionOrderId),
                        Type = FileTypeEnum.Tag
                    });
                    break;
            }

            if (file.Count > 0)
            {
                var item = await _zplToPdfService.ConvertFilesToPDFs(file);

                var pdfMarged = _zplToPdfService.MargeSimplifiedDanfesPdfsFiles(item);

                return pdfMarged.GetByteByPdfDocument();
            }

            return null;
        }

        public async Task<ExpeditionOrder> SetVolumeQuantity(string invoiceAccessKey, int volumeQuantities)
        {
            var order = await _expeditionOrderRepository.GetOrderByInvoiceAccessKeyAsync(invoiceAccessKey);

            order.VolumeQuantity = volumeQuantities;

            if (order.Status != ExpeditionOrderStatus.Packed && order.Status != ExpeditionOrderStatus.Dispatched)
            {
                order.Status = ExpeditionOrderStatus.Packed;

                var packing = await _packingRepository.GetByExpeditionOrderIdAsync(order.Id, false);

                foreach (var item in packing.Items)
                {

                    var product = await _productRepository.GetByIdAsync(Convert.ToInt32(item.ProductId));

                    _productService.RemoveReservedStockProduct(product, Convert.ToDouble(item.Quantity), OriginInventoryMovimentEnum.ExpeditionOrder, order.IssueDate, "Removendo estoque reservado para adicionar no volume");
                }
            }

            _expeditionOrderRepository.Update(order);

            return order;
        }

        public async Task<List<ExpeditionOrderGroup>> GetGroupOfOrdersWithIds()
        {
            var orders = await GetOrdersIsNoGeneratedReturnInvoiceGroupedByClient();

            List<ExpeditionOrderGroup> orderGroups = new List<ExpeditionOrderGroup>();

            foreach (var order in orders)
            {
                ExpeditionOrderGroup orderGroup = new ExpeditionOrderGroup()
                {
                    ClientName = order.Orders.First().ClientName,
                    Cnpj = order.Orders.First().Cnpj,
                    OrdersQuantity = order.Orders.Count(),
                    TotalValue = GetTotalValueInListOfExpeditionOrders(order.Orders),
                    Orders = order.Orders.Select(e => e.Id).ToList(),
                    ClientId = order.ClientId
                };

                orderGroups.Add(orderGroup);
            }

            return orderGroups;
        }

        public decimal? GetTotalValueInListOfExpeditionOrders(List<ExpeditionOrder> orders)
        {
            return orders.Sum(e => e.InvoiceValue);
            /*decimal? totalValue = 0;
            foreach (var order in orders)
            {
                List<ExpeditionOrderItem> itens = order.ExpeditionOrderItems;
                foreach (var item in itens)
                {
                    var itemValue = item.Value * item.Quantity;
                    totalValue += itemValue;
                }
            }

            return totalValue;*/
        }

        public async Task<ExpeditionOrder> AddOrderAndUpdateInvoice(ExpeditionOrder order)
        {
            ExpeditionOrder exOrder = await _expeditionOrderRepository.Add(order);

            Invoice invoice = await _invoiceRepository.GetByAcessKeyAsync(order.InvoiceAccessKey);

            invoice.ExpeditionOrderId = exOrder.Id;
            _invoiceRepository.Update(invoice);

            return exOrder;
        }

        public async Task Approve(int id)
        {
            try
            {
                ExpeditionOrder order = await _expeditionOrderRepository.GetOrderByIdAsync(id);

                string message = $"<b>Série da Nota: {order.InvoiceSerie} / Nro. Nota: {order.InvoiceNumber} </b>";
                string emailTitle = $"<b>Série da Nota: {order.InvoiceSerie} / Nro. Nota: {order.InvoiceNumber} </b>";
                string email = $"<b>Série da Nota: {order.InvoiceSerie} / Nro. Nota: {order.InvoiceNumber} </b>";
                string databaseMessage = $"";

                bool enviarEmail = false;

                bool existeErro = false;

                // execute validation
                foreach (var item in order.ExpeditionOrderItems)
                {
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {

                        Product product = null;

                        if (item.ProductId == 0 || item.ProductId == null)
                        {
                            product = await _productRepository.GetByEanAsync(item.Ean, Convert.ToInt32(order.ClientId));

                            if (product == null)
                                product = await _productRepository.GetByCodeAsync(item.ExternalNumberItem, Convert.ToInt32(order.ClientId));

                            if (product != null)
                            {
                                item.ProductId = product.Id;
                                await _expeditionOrderItemsRepository.UpdateAsync(item);

                            }
                            else
                            {
                                message = string.Format($"{message} <br> {item.Ean} {item.Name} não está vinculado no sistema");
                                databaseMessage = string.Format($" | {databaseMessage} {item.Ean} {item.Name} não está vinculado no sistema");
                                existeErro = true;
                            }
                        }
                        else
                        {
                            product = await _productRepository.GetByIdAsync(Convert.ToInt32(item.ProductId));
                        }

                        if (product != null)
                        {
                            if (product.StockQuantity < Convert.ToDouble(item.Quantity))
                            {

                                message = string.Format($"{message} <br> {item.Ean} {item.Name} não possui estoque suficiente.");
                                email = string.Format($"{email} <br> {item.Ean} {item.Name} não possui estoque suficiente.");
                                databaseMessage = string.Format($"| {databaseMessage} {item.Ean} {item.Name} não possui estoque suficiente.");
                                existeErro = true;
                                enviarEmail = true;
                            }
                        }
                        else
                        {
                            await transaction.RollbackAsync();
                            throw new Exception($"Produto {item.Name} não encontrado no nosso estoque.");
                        }

                        await transaction.CommitAsync();
                    }
                }

                if (existeErro)
                {
                    if (enviarEmail)
                    {
                        EmailData emailData = new EmailData()
                        {
                            EmailToId = order.Client.Email,
                            EmailToName = order.Client.SocialReason,
                            EmailSubject = emailTitle,
                            EmailBody = email
                        };

                        _emailService.SendEmail(emailData, null, order.ClientId, order.InvoiceNumber);

                        order.Errors = databaseMessage;

                        _expeditionOrderRepository.Update(order);
                    }

                    throw new ProblemWithAcceptOrderException(string.Format($"<pre>{message}</pre>"), order.Id);
                }



                // if there are no errors
                foreach (var item in order.ExpeditionOrderItems)
                {
                    Thread.Sleep(200);
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        var product = await _productRepository.GetByIdAsync(Convert.ToInt32(item.ProductId));
                        _productService.AddReservedStockFromStockProduct(product, Convert.ToDouble(item.Quantity), OriginInventoryMovimentEnum.ExpeditionOrder, order.IssueDate, $"O produto está sendo adicionado na reserva pela aprovação da nota {order.InvoiceNumber} - {order.InvoiceAccessKey}");

                        await transaction.CommitAsync();
                    }
                }

                User user = _userRepository.GetUserLoged();

                await UpdateOrder(id, ExpeditionOrderStatus.Processed);
                await _expeditionOrderHistoryService.Add(id, "Pedido processado", ExpeditionOrderStatus.Processed, user.Id);


                if(order.OrderOrigin != OrderOrigin.ClientPanel && order.OrderOrigin != OrderOrigin.XMLCreation && order.ExpeditionOrderTagShipping == null)
                {
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        await ProcessTag(order);

                        await transaction.CommitAsync();
                    }
                }
            }
            catch (ProblemWithAcceptOrderException ex)
            {
                throw new ProblemWithAcceptOrderException(ex.Message, id);
            }

            catch (Exception ex)
            {
                throw new ProblemWithAcceptOrderException(ex.Message, id);
            }
        }

        public async Task Cancel(int id, string subject, string? messageSended)
        {
            //Get ExpeditionOrder an Items
            ExpeditionOrder order = await _expeditionOrderRepository.GetOrderByIdAsync(id);
            List<ExpeditionOrderItem> orderItems = await _expeditionOrderItemsRepository.GetByOrderIdAsync(order.Id);

            string messageError = $"<b>Pedido: {order.InvoiceNumber}</b>";

            bool existeErro = false;

            //If can't find order
            if (order == null)
            {
                messageError = string.Format($"O pedido selecionado não foi encontrado");
                existeErro = true;
            }

            if (order.OrderOrigin == OrderOrigin.XMLCreation)
            {
                var invoice = await _invoiceService.GetByAcessKeyAsync(order.InvoiceAccessKey);
                if (invoice != null)
                {
                    _invoiceService.DeleteById(invoice.Id);
                }

                order.InvoiceAccessKey = "";
            }

            Packing packing = _packingRepository.GetByExpeditionOrderId(order.Id);

            //Cancel packing if exists
            if (packing != null)
            {
                decimal quantityOrderItemSum = orderItems.Sum(x => x.Quantity);
                packing.Quantity -= quantityOrderItemSum;
                await _packingRepository.UpdateStatusAsync(packing, PackingStatus.Cancelado);
                await _packingItemRepository.UpdatePackingItemByStatusAsync(packing.Id, PackingItemStatus.Cancelado);
            }

            //Cancel picking if exists
            if (order.PickingListId != null || order.PickingListId > 0)
            {
                PickingList picking = await _pickingListRepository.GetByIdAsync(order.PickingListId);

                List<PickingListItem> pickingListItem = _pickingListItemRepository.GetByPickingListId(picking.Id);

                foreach (var orderItem in orderItems)
                {
                    int orderItemQuantity = Decimal.ToInt32(orderItem.Quantity);

                    PickingListItem itemSelected = pickingListItem.Where(x => x.ProductId == orderItem.ProductId).Where(x => x.Quantity > 0).FirstOrDefault();

                    if (orderItemQuantity > itemSelected.Quantity)
                    {
                        itemSelected.ItemStatus = PickingListItemStatus.Cancelado;

                    }
                    else
                    {
                        itemSelected.Quantity -= Decimal.ToInt32(orderItem.Quantity);
                    }

                    _pickingListItemRepository.Update(itemSelected);
                }

                bool allItemsHaveSameStatusCanceled = pickingListItem.All(item => item.ItemStatus == PickingListItemStatus.Cancelado);

                if (allItemsHaveSameStatusCanceled)
                {
                    picking.Status = PickingListStatus.Cancelado;
                    _pickingListRepository.Update(picking);
                }
            }
            
            if(order.Status != ExpeditionOrderStatus.ProcessingPendenting)
            {
                //taking items from reserve and adding to stock
                foreach (var orderItem in orderItems)
                {
                    double orderItemQuantity = Decimal.ToDouble(orderItem.Quantity);

                    if (orderItem.ProductId > 0)
                    {
                        Product product = await _productRepository.GetByIdAsync(orderItem.ProductId);

                        _productService.AddReservedStockToStockProduct(product, orderItemQuantity, OriginInventoryMovimentEnum.ExpeditionOrder, order.IssueDate, $"O produto está removido da reserva pelo cancelamento da nota {order.InvoiceNumber} - {order.InvoiceAccessKey}");

                    }

                }
            }

            //Updating order to canceled
            await UpdateOrder(order.Id, ExpeditionOrderStatus.Canceled);

            //Adding orderNumber to message
            var message = $"<b>Pedido: {order.InvoiceNumber}</b><br>" + messageSended;

            //Sending Email
            _emailService.ReceiveMessageAndSendEmail(subject, message, null, order.ClientId);
        }



        public async Task<List<string>> GenerateMelhorEnvioUrl(List<ExpeditionOrder> list)
        {
            // GroupBy ClientId
            var gruposPorCliente = list.GroupBy(x => x.ClientId);

            List<string> response = new List<string>();

            foreach (var grupo in gruposPorCliente)
            {
                // Checks if the group contains at least one MELHOR ENVIO order
                bool containsME = grupo.Any(order => order.ShippingMethod == ShippingMethodEnum.MelhorEnvio);

                if (containsME)
                {
                    List<ExpeditionOrderTagShipping> orders = grupo
                    .Where(order => order.ShippingMethod == ShippingMethodEnum.MelhorEnvio)
                     .Select(order => order.ExpeditionOrderTagShipping).ToList();

                    var ordersId = orders.Select(e => e.ShippingCode).ToList();

                    int clientId = grupo.Key ?? 0;


                    // Get the shipping url for MELHOR ENVIO order group
                    GetShippingDataMelhorEnvioDtoResponse data = await _melhorEnvioAPIServices.GetShippingData(ordersId, clientId);

                    if (data?.Url != null)
                    {
                        response.Add(data.Url);
                    }
                    else
                    {
                        if (orders.Count == 1)
                        {
                            response.Add(orders[0].Url);
                        }
                    }
                }
            }

            return response;
        }

        public async Task<byte[]> GenerateSimplifiedDanfe(List<int> list)
        {

            var expeditionOrders = await _expeditionOrderRepository.GetAllById(list);

            Log.Info(string.Format("Pedidos encontrados {0}", string.Join('|', expeditionOrders.Select(e => e.Id))));

            //string url = await GenerateMelhorEnvioUrl(expeditionOrders);

            var generatedFiles = await GenerateFilesConvert(expeditionOrders);

            var data = await _zplToPdfService.ConvertFilesToPDFs(generatedFiles);

            var pdfMarged = _zplToPdfService.MargeSimplifiedDanfesPdfsFiles(data);

            byte[] byteData = pdfMarged.GetByteByPdfDocument();


            return byteData;
        }

        public async Task<List<FileConvert>> GenerateFilesConvert(List<ExpeditionOrder> list)
        {
            Log.Info(string.Format("Iniciando geração de danfe"));
            var baseDanfeFilePath = Environment.ZplConfiguration.BaseSimplifiedDanfe;


            var danfeBaseFileContent = await File.ReadAllTextAsync(baseDanfeFilePath);

            List<FileConvert> generaterdFiles = new List<FileConvert>();

            foreach (var expeditionOrder in list)
            {
                try
                {
                    Log.Info(string.Format("Buscando nota do pedido {0}.", expeditionOrder.Id));
                    var invoice = await _invoiceService.GetByAcessKeyAsync(expeditionOrder.InvoiceAccessKey);

                    string danfe = null;

                    if (invoice != null)
                    {
                        Log.Info(string.Format("Gerando danfe com base no XML. Pedido {0}", expeditionOrder.Id));
                        danfe = await _simplifiedDanfeService.GenerateSimplifiedDanfeByReceiptNote(danfeBaseFileContent, invoice);
                    }

                    if (danfe == null)
                    {
                        Log.Info(string.Format("Gerando danfe com base nos dados do pedido. Pedido {0}", expeditionOrder.Id));
                        danfe = _simplifiedDanfeService.GenerateSimplifiedDanfe(danfeBaseFileContent, expeditionOrder, expeditionOrder.Client);
                    }

                    Log.Info(string.Format("Adicionando danfe no fluxo. Pedido {0}", expeditionOrder.Id));
                    generaterdFiles.Add(new FileConvert()
                    {
                        OrderNumber = expeditionOrder.Id,
                        Content = danfe,
                        Format = FileFormatEnum.Zpl,
                        Type = FileTypeEnum.SimplifiedDanfe
                    });

                    string content = null;

                    if (expeditionOrder.ExpeditionOrderTagShipping != null)
                    {
                        switch (expeditionOrder.ExpeditionOrderTagShipping.FileFormat)
                        {
                            case FileFormatEnum.Zpl:
                                Log.Info(string.Format("Processando etiqueta no formato ZPL. Pedido {0}", expeditionOrder.Id));
                                content = await expeditionOrder.ExpeditionOrderTagShipping.Url.GetZplFileContentAsync();
                                break;
                            case FileFormatEnum.Pdf:
                                Log.Info(string.Format("Processando etiqueta no formato URL / PDF. Pedido {0}", expeditionOrder.Id));
                                content = expeditionOrder.ExpeditionOrderTagShipping.Url;
                                break;
                            case FileFormatEnum.Image:
                                Log.Info(string.Format("Processando etiqueta no formato URL / Imagem. Pedido {0}", expeditionOrder.Id));
                                content = expeditionOrder.ExpeditionOrderTagShipping.Url;
                                break;
                            case FileFormatEnum.Url:
                                break;
                        }
                    }

                    if (content != null)
                    {
                        Log.Info(string.Format("Etiqueta gerada, adicionando no fluxo. Pedido {0}", expeditionOrder.Id));
                        generaterdFiles.Add(new FileConvert()
                        {
                            OrderNumber = expeditionOrder.Id,
                            Content = content,
                            Format = expeditionOrder.ExpeditionOrderTagShipping.FileFormat,
                            Type = FileTypeEnum.Tag
                        });
                    }
                }
                catch (Exception ex)
                {
                    Log.Info(string.Format("Não foi possivel gerar a etiqueta. Pedido {0}, Message {1} - {2}", expeditionOrder.Id, ex.Message, ex.StackTrace));
                }
            }

            Log.Info(string.Format("Geração de etiquetas finalizadas. Pedido {0}", string.Join(',', list.Select(e => e.Id))));
            return generaterdFiles;
        }

        public async Task<List<ExpeditionOrder>> GetOrdersByPickinIds(List<int> ids)
        {
            List<ExpeditionOrder> melhorEnvioOrder = new();
            foreach (int id in ids)
            {
                PickingList picking = await _pickingListRepository.GetByIdAsync(id);

                foreach (var order in picking.ExpeditionOrder)
                {
                    if (order.ShippingMethod == ShippingMethodEnum.MelhorEnvio)
                    {
                        melhorEnvioOrder.Add(order);
                    }
                }
            }

            return melhorEnvioOrder;
        }

        public async Task<List<List<string>>> CheckIfOrderIsMelhorEnvio(List<int> ids)
        {
            Dictionary<int, List<string>> clientIdToOrders = new Dictionary<int, List<string>>();


            List<ExpeditionOrder> orderGet = await _expeditionOrderRepository.GetAllById(ids);

            if (orderGet == null)
            {
                throw new Exception($"Não foi possível encontrar a separação de número dos pedidos escolhidos");
            }

            foreach (var order in orderGet)
            {
                ExpeditionOrderTagShipping tag = await _expeditionOrderTagShippingRepository.GetShippingByExpeditionOrderIdAsync(order.Id);

                if (tag != null && order.ShippingMethod == ShippingMethodEnum.MelhorEnvio)
                {
                    if (!clientIdToOrders.ContainsKey(order.ClientId ?? 0))
                    {
                        clientIdToOrders[order.ClientId ?? 0] = new List<string>();
                    }

                    clientIdToOrders[order.ClientId ?? 0].Add(order.Id.ToString());
                }
            }


            return clientIdToOrders.Values.ToList();
        }

        public async Task<PaginationBase<ExpeditionOrderWithPickingListViewModel>> GetAllManualOrders(ExpeditionOrderFilterViewModel? request)
        {
            PaginationBase<ExpeditionOrderWithPickingListViewModel> pagination = new();

            PaginationBase<ExpeditionOrder> paginationOrder = await _expeditionOrderRepository.GetAllManualOrdersAsync(request);

            pagination = await ConvertOrderExpeditionToViewModel(paginationOrder);

            return pagination;
        }



        public async Task<List<ExpeditionOrder>> GetPendentingReturnOrdersByClientId(int clientId)
        {
            List<ExpeditionOrder> orders = await _expeditionOrderRepository.GetOrdersIsNotGeneratedReturnInvoiceByClientId(clientId);            

            var returnOrders = await AddNotFoundedPendingOrdersByClientId(orders, clientId);

            return returnOrders;
        }

        public async Task<List<ClientOrdersViewModel>> GetOrdersIsNoGeneratedReturnInvoiceGroupedByClient()
        {
            return await GetOrdersIsNoGeneratedReturnInvoiceGroupedByClientAndDate(DateTime.Now);            
        }


        public async Task<List<ClientOrdersViewModel>> GetOrdersIsNoGeneratedReturnInvoiceGroupedByClientAndDate(DateTime maxDate)
        {
            List<ExpeditionOrder> orders = await _expeditionOrderRepository.GetOrdersIsNotGeneratedReturnInvoice(maxDate);

            List<ClientOrdersViewModel> returnOrders = ConvertListToClientsOrderView(orders);

            returnOrders = await AddNotFoundedPendingOrders(returnOrders);

            return returnOrders;
        }

        private async Task<List<ClientOrdersViewModel>> AddNotFoundedPendingOrders(List<ClientOrdersViewModel> returnOrders)
        {
            foreach (var order in returnOrders)
            {
                order.Orders = await AddNotFoundedPendingOrdersByClientId(order.Orders, order.ClientId);                
            }

            return returnOrders;
        }

        private async Task<List<ExpeditionOrder>> AddNotFoundedPendingOrdersByClientId(List<ExpeditionOrder> orders, int clientId)
        {
            List<ExpeditionOrdersLotNotFounded> expeditionOrderLotsNotFounded = await _expeditionOrdersLotNotFoundedRepository.GetByClientIdAndStatus(clientId, ExpeditionOrdersLotNotFoundedStatusEnum.Pendenting);

            var lotsNotFoundedByExpeditionOrders = expeditionOrderLotsNotFounded.GroupBy(e => e.ExpeditionOrderId);

            foreach (var expeditionNotFounded in lotsNotFoundedByExpeditionOrders)
            {
                var expeditionOrder = expeditionNotFounded.First().ExpeditionOrder;

                var ordersItems = expeditionOrder.ExpeditionOrderItems;

                expeditionOrder.ExpeditionOrderItems = new List<ExpeditionOrderItem>();

                foreach (var item in expeditionNotFounded)
                {
                    var product = ordersItems.Where(e => e.ProductId == item.ProductId).First();

                    expeditionOrder.ExpeditionOrderItems.Add(new ExpeditionOrderItem()
                    {
                        Description = product.Description,
                        ExpeditionOrderId = expeditionOrder.Id,
                        Quantity = item.Quantity,
                        Value = product.Value,
                        Ean = product.Ean,
                        Name = product.Name,
                        ExternalNumberItem = product.ExternalNumberItem,
                        ProductId = product.Id,
                        Id = 0
                    });
                }

                orders.Add(expeditionOrder);
            }

            return orders;
        }

        private List<ClientOrdersViewModel> ConvertListToClientsOrderView(List<ExpeditionOrder> orders)
        {
            var groupedOrders = orders.GroupBy(x => x.ClientId);

            var itemObjs = new List<ClientOrdersViewModel>();

            foreach (var item in groupedOrders)
            {
                itemObjs.Add(new ClientOrdersViewModel()
                {
                    ClientId = Convert.ToInt32(item.Key),
                    Orders = item.ToList()
                });
            }

            return itemObjs;
        }

        public List<int> GetTagBlockedOrderIds()
        {
            var startDate = DateTimeHelper.GetCurrentDateTime().Date.AddDays(-1);
            var orderIdsToProcessTag = _expeditionOrderRepository.GetQueryableFilter(new ExpeditionOrderFilterViewModel
            {
                ShippingTagBlocked = 1, // search by blocked tag>,
                CreationStartDate = startDate
            }).Select(eo=>eo.Id).ToList();

            return orderIdsToProcessTag;
        }

        public async Task<bool> CheckIfOrderExistsByInvoiceNumberAndClientId(int id, int invoiceNumber, int clientId)
        {
            ExpeditionOrder order = await _expeditionOrderRepository.GetOrderByInvoiceNumberAndClientId(id, invoiceNumber, clientId);

            if(order == null)
            {
                return false;
            }

            return true;
        }
    }

}
