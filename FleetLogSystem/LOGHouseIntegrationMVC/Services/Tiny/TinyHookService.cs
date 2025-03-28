using LOGHouseSystem.Adapters.Extensions.TinyExtension.Dto.Hook.Request;
using LOGHouseSystem.Adapters.Extensions.TinyExtension.Service;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Models;
using LOGHouseSystem.Models.SendEmails;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Tiny.Dto;
using Newtonsoft.Json.Linq;
using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.Services.Helper;
using Newtonsoft.Json;
using LOGHouseSystem.Controllers;
using Org.BouncyCastle.Asn1.Ocsp;

namespace LOGHouseSystem.Services
{
    public class TinyHookService : ITinyHookService
    {
        private IEmailService _emailService;
        private ITinyOrdersRepository _tinyOrdersRepository;
        private IClientsRepository _clientsRepository;
        private ITinyAPIService _tinyAPIService;
        private IProductRepository _productRepository;
        private IExpeditionOrderRepository _expeditionOrderRepository;
        private IExpeditionOrderService _expeditionOrderService;
        private INFeService _nFeService;
        private IIntegrationVariableRepository _integrationVariableRepository;
        private AppDbContext _context;
        private IHookInputService _hookInputService;
        private IInvoiceService _invoiceService;
        private IIntegrationRepository _integrationRepository;
        private readonly IExpeditionOrderHistoryService _expeditionOrderHistoryService;

        public TinyHookService(IEmailService emailService, 
            ITinyOrdersRepository tinyOrdersRepository, 
            IClientsRepository clientsRepository,
            ITinyAPIService tinyAPIService,
            IProductRepository productRepository,
            IExpeditionOrderRepository expeditionOrderRepository,
            IIntegrationVariableRepository integrationVariableRepository,
            IExpeditionOrderService expeditionOrderService,
            INFeService nFeService,
            AppDbContext context,
            IHookInputService hookInputService,
            IIntegrationRepository integrationRepository,
            IInvoiceService invoiceService,
            IExpeditionOrderHistoryService expeditionOrderHistoryService
            )
        {
            _emailService = emailService;
            _tinyOrdersRepository = tinyOrdersRepository;
            _clientsRepository = clientsRepository;
            _tinyAPIService = tinyAPIService;
            _productRepository = productRepository;
            _context = context;
            _expeditionOrderRepository = expeditionOrderRepository;
            _integrationVariableRepository = integrationVariableRepository;
            _expeditionOrderService = expeditionOrderService;
            _nFeService = nFeService;
            _hookInputService = hookInputService;
            _integrationRepository = integrationRepository;
            _invoiceService = invoiceService;
            _expeditionOrderHistoryService = expeditionOrderHistoryService;

        }


        public async Task ReceiveOnder(TinyInvoiceWebhookRequest webhook)
        {
            // Send email notifing webhook received
            SaveLog(webhook);

            // Get the client
            Client client = await _clientsRepository.FindByCnpjAsync(webhook.Cnpj);

            // verify if client was founded
            // The client is necessary because the hook needs to get the complete order from Tiny order API and the token needs to be returned by the database.
            if (client == null)
            {
                throw new Exception(string.Format("Não foi possivel encontrar o cliente {0} na base. É necessário que o cliente esteja cadastrado para que sejá possível buscar o pedido completo", webhook.Cnpj));
            }            

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var invoice = await _tinyAPIService.GetInvoice(webhook.Dados.IdNotaFiscalTiny.ToString(), client);

                    if(invoice.retorno.erros != null && invoice.retorno.erros.Any())
                    {
                        string errorMessages = string.Join(',',invoice.retorno.erros.Select(err=>err.erro).ToList());

                        throw new Exception($"Falha ao buscar NF: {errorMessages}");
                    }

                    if (string.IsNullOrEmpty(invoice.retorno.nota_fiscal.id_venda.ToString()))
                    {
                        throw new Exception("Id do pedido não está presente na NF");
                    }

                    // Get complete order with products
                    TinyCompleteOrderRequestDto order = await _tinyAPIService.GetOrder(invoice.retorno.nota_fiscal.id_venda.ToString(), client);

                    // Get XML Note           
                    var nfe = await _tinyAPIService.GetXmlInvoice(webhook.Dados.IdNotaFiscalTiny, client);

                    int invoiceId = 0;

                    if (nfe == null)
                    {
                        //Get LOGHouse product id
                        //order = await FillCompleteProducts(order, webhook, client);
                        throw new Exception("Não é possível inserir um pedido sem nota fiscal.");
                    }
                    else
                    {
                        var responseDto = await _nFeService.ImportNfeAsync(nfe,0, 1);
                    }

                    // Convert to model
                    var orderEntity = await ConvertRequestToModel(order, webhook, client);

                    bool checkIfOrderAlreadyExist = await _expeditionOrderService.CheckIfOrderExistsByExternalNumber(orderEntity, OrderOrigin.Tiny);

                    if (checkIfOrderAlreadyExist)
                    {
                        return;
                    }

                    ExpeditionOrder orderReceived = await _expeditionOrderService.AddOrderAndUpdateInvoice(orderEntity);
                    await _expeditionOrderHistoryService.Add(orderReceived.Id, "Pedido Gerado - Tiny", ExpeditionOrderStatus.ProcessingPendenting, 6);

                    await transaction.CommitAsync();

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<TinyCompleteOrderRequestDto> FillCompleteProducts(TinyCompleteOrderRequestDto order, TinyInvoiceWebhookRequest webhook, Client client)
        {

            //invoice
            var invoice = await _invoiceService.GetByAcessKeyAsync(webhook.Dados.ChaveAcesso);

            foreach (var item in order.Retorno.Pedido.Itens)
            {               

                Product product = null;
                
                // try take product gtin by XML note
                var invoiceItem = invoice?.InvoiceItems?.Where(e => e.Code == item.Item.Codigo.ToString()).FirstOrDefault();

                if (invoiceItem != null)
                {
                    if (invoiceItem != null)
                    {
                        item.Gtin = invoiceItem.Ean;
                        product = await _productRepository.GetByEanAsync(item.Gtin, client.Id);
                        if (product != null)
                        {
                            item.ProductId = product.Id;
                        }                        
                    }
                }
                else
                {
                    // if wasn't founded, try to take from API service
                    var completeProduct = await _tinyAPIService.GetProduct(item.Item.TinyId, client);

                    if (completeProduct != null)
                    {

                        // Try to take the product by package ean
                        if (!string.IsNullOrEmpty(completeProduct.Retorno.Produto.GtinEmbalagem) && completeProduct.Retorno.Produto.GtinEmbalagem != completeProduct.Retorno.Produto.Gtin)
                        {
                            product = await _productRepository.GetByEanAsync(completeProduct.Retorno.Produto.GtinEmbalagem, client.Id);
                        }

                        // If there arent product, (maybe because by package ean didn't found)
                        // Try to take the product by ean product
                        if (product == null)
                        {
                            if (!string.IsNullOrEmpty(completeProduct.Retorno.Produto.Gtin))
                            {
                                product = await _productRepository.GetByEanAsync(completeProduct.Retorno.Produto.Gtin, client.Id);
                            }
                        }

                        // If was founded product by ean product or ean package, take the product id to make foreigh key
                        if (product != null)
                        {
                            item.ProductId = product.Id;
                            item.Gtin = product.Ean;
                        }
                        else
                        {
                            // if wasn't founded a product, try to fill gtin to model
                            if (!string.IsNullOrEmpty(completeProduct.Retorno.Produto.GtinEmbalagem))
                            {
                                item.Gtin = completeProduct.Retorno.Produto.GtinEmbalagem;
                            }
                            else if (!string.IsNullOrEmpty(completeProduct.Retorno.Produto.Gtin))
                            {
                                item.Gtin = completeProduct.Retorno.Produto.Gtin;
                            }
                        }
                    }
                }                
            }

            return order;
        }

        private  async Task<ExpeditionOrder> ConvertRequestToModel(TinyCompleteOrderRequestDto order, TinyInvoiceWebhookRequest webhook, Client client)
        {
            var expeditionOrder = new ExpeditionOrder()
            {
                ClientName = client.SocialReason,
                Cnpj = webhook.Cnpj,
                ExternalNumber = order.Retorno.Pedido.Id.ToString(),
                InvoiceAccessKey = webhook.Dados.ChaveAcesso,
                OrderOrigin = OrderOrigin.Tiny,
                IssueDate = webhook.Dados.DataEmissao,
                ShippingCompany = order.Retorno.Pedido.NomeTransportador,
                Obs = order.Retorno.Pedido.Obs,
                ClientId = client != null ? client.Id : null,
                Status = ExpeditionOrderStatus.ProcessingPendenting,                
                InvoiceNumber = Convert.ToInt32(webhook.Dados.Numero),
                InvoiceValue = Decimal.Parse(order.Retorno.Pedido.TotalPedido),
                InvoiceSerie = webhook.Dados.Serie,
            };

            expeditionOrder.ShippingMethod = await order.Retorno.Pedido.Ecommerce.NomeEcommerce.GetShippingMethod(client, _integrationRepository);

            if (expeditionOrder.ShippingMethod != ShippingMethodEnum.Kangu)
            {
                
                expeditionOrder.ShippingMethodCodeOrder = order.Retorno.Pedido.Ecommerce.NumeroPedidoEcommerce;
            }
            else
            {                
                expeditionOrder.ShippingMethodCodeOrder = order.Retorno.Pedido.CodigoRastreamento;
            }

            if (order != null)
            {
                //invoice
                var invoice = await _invoiceService.GetByAcessKeyAsync(webhook.Dados.ChaveAcesso);

                var cliente = order.Retorno.Pedido.Cliente;

                expeditionOrder.ShippingDetails = new ShippingDetails()
                {
                    Name = cliente.Nome,
                    FantasyName = cliente.NomeFantasia,
                    CpfCnpj = cliente.CpfCnpj,
                    Rg = cliente.Rg,
                    Address = cliente.Endereco,
                    Number = cliente.Numero.ToString(),
                    Complement = cliente.Complemento,
                    Neighborhood = cliente.Bairro,
                    Cep = cliente.Cep,
                    City = cliente.Cidade,
                    Uf = cliente.Uf,
                    Phone = client.Phone
                 };

                expeditionOrder.ShippingDetailsId = expeditionOrder.ShippingDetails.Id;
                
                // there are null ean itens                

                if (invoice != null)
                {
                    expeditionOrder.ExpeditionOrderItems = new List<ExpeditionOrderItem>();

                    foreach (var e in invoice.InvoiceItems)
                    {

                        var exItem =  new ExpeditionOrderItem()
                        {
                            Name = e.Description,
                            Quantity = Convert.ToDecimal(e.Quantity),
                            Description = e.Description,
                            ExternalNumberItem = e.Code,
                            Value = e.Value,
                            Ean = e.Ean,
                            ExpeditionOrderId = expeditionOrder.Id
                        };

                        /*Product product = new Product()
                        {
                            Code = exItem.ExternalNumberItem,
                            Description = exItem.Description,
                            Ean = exItem.Ean,
                            StockQuantity = Convert.ToDouble(exItem.Quantity),
                            CreatedAt = DateTime.Now,
                            ClientId = client.Id
                        };

                        Product productReturned = _productService.CheckAndAddProduct(product.Ean, product);*/

                        Product productReturned = await _productRepository.GetByEanAsync(exItem.Ean, client.Id);

                        if (productReturned != null) { exItem.ProductId = productReturned.Id; }

                        expeditionOrder.ExpeditionOrderItems.Add(exItem);
                    }
                }
                else
                {
                    expeditionOrder.ExpeditionOrderItems = order.Retorno.Pedido.Itens.Select(e => new ExpeditionOrderItem()
                    {
                        Name = e.Item.Codigo,
                        Quantity = e.Item.Quantidade,
                        Description = e.Item.Descricao,
                        ExternalNumberItem = e.Item.TinyId.ToString(),
                        Value = e.Item.ValorUnitario,
                        Ean = e.Gtin,
                        ProductId = e.ProductId,
                        ExpeditionOrderId = expeditionOrder.Id

                    }).ToList();
                }
                
            }

            return expeditionOrder;
        }

        public void SaveLog(TinyInvoiceWebhookRequest order)
        {
            JObject newClient = JObject.FromObject(order);

            Log.Info($"Novo pedido Tiny | Novo pedido recebido no webhook de nota fiscal da Tiny. \n\n Payload: {newClient}");
        }

        public async Task CreateIntegrationVariables(int integrationId)
        {
            var integrationVariables = new IntegrationVariable
            {
                IntegrationId = integrationId,
                Name = TinyIntegationNames.ApiKey,
                Value = ""
            };

            await _integrationVariableRepository.AddAsync(integrationVariables);
        }

        public async Task ProcessTinyOrder(HookInput order)
        {
            TinyInvoiceWebhookRequest tinyObject = JsonConvert.DeserializeObject<TinyInvoiceWebhookRequest>(order.Payload);

            await ReceiveOnder(tinyObject);

            await _hookInputService.DeleteById(order.Id);
        }
    }
   
}