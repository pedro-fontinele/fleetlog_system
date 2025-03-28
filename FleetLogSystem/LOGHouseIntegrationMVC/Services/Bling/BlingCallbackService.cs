using LOGHouseSystem.Adapters.Extensions.BlingExtension;
using LOGHouseSystem.Adapters.Extensions.BlingExtension.Dto.Hook.Request;
using LOGHouseSystem.Adapters.Extensions.Discord;
using LOGHouseSystem.Adapters.Extensions.NFeExtension;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Helper;
using LOGHouseSystem.Services.Interfaces;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Web;

namespace LOGHouseSystem.Services
{
    public class BlingCallbackService : IBlingCallbackService
    {        
        private IClientsRepository _clientsRepository;
        private IProductRepository _productRepository;
        private IIntegrationVariableRepository _integrationVariableRepository;
        private IExpeditionOrderService _expeditionOrderService;
        private IBlingExtensionService _blingExtentionService;
        private IIntegrationRepository _integrationRepository;
        private INFeService _nFeService;
        private AppDbContext _context;
        private IHookInputService _hookInputService;
        private INFeExtension _nFeExtension;
        private readonly IInvoiceService _invoiceService;
        private readonly IExpeditionOrderHistoryService _expeditionOrderHistoryService;
        private readonly IRetryQueueService _retryQueueService;

        public BlingCallbackService(
            IClientsRepository clientsRepository,
            IProductRepository productRepository,
            IIntegrationVariableRepository integrationVariableRepository,
            IExpeditionOrderService expeditionOrderService,
            IBlingExtensionService blingExtentionService,
            IIntegrationRepository integrationRepository,
            INFeService nFeService,                    
            AppDbContext context,
            IHookInputService hookInputService,
            INFeExtension nFeExtension,
            IInvoiceService invoiceService, 
            IExpeditionOrderHistoryService expeditionOrderHistoryService,
            IRetryQueueService retryQueueService)
        {
            _clientsRepository = clientsRepository;
            _productRepository = productRepository;
            _context = context;
            _integrationVariableRepository = integrationVariableRepository;
            _expeditionOrderService = expeditionOrderService;
            _blingExtentionService = blingExtentionService;
            _integrationRepository = integrationRepository;
            _nFeService = nFeService;
            _hookInputService = hookInputService;
            _nFeExtension = nFeExtension;
            _invoiceService = invoiceService;
            _expeditionOrderHistoryService = expeditionOrderHistoryService;
            _retryQueueService = retryQueueService;
        }


        public async Task ReceiveOrder(BlingPedidoCallbackRequest pedido, string xmlOrder, string? cnpj = null)
        {
            Client? client = null;
            DiscordExtension discordExtension = new DiscordExtension();

            // Save log
            SaveLog(pedido);

            // Try to take the client by CNPJ
            if (!string.IsNullOrEmpty(cnpj))
            {
                client = await _clientsRepository.FindByCnpjAsync(cnpj);
            }

            if (client == null)
            {                
                throw new Exception("Cliente não encontrado na base");
            }

            if (pedido.Nota != null)
            {
                
                // Open transaction
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        (_, var invoiceXml) = await DownloadXmlFileByUrl(xmlOrder, client.Id);

                        var nfeFormated = _nFeExtension.DeserializeNFe(invoiceXml);

                        //Import Invoice
                        var responseDto = await _nFeService.ImportNfeAsync(invoiceXml, 0, 1);
                        if (!responseDto.Success)
                        {
                            throw new Exception(responseDto.Message);
                        }

                        pedido.Nota.ChaveAcesso = nfeFormated.ProtNFe.InfProt.ChNFe;
                        // Generate model by dto            
                        ExpeditionOrder expeditionOrder = await GenerateBlingOrderDtoToModel(pedido, client, nfeFormated);

                        //Check if Order already exist
                        bool checkResult = await _expeditionOrderService.CheckIfOrderExistsByExternalNumber(expeditionOrder, OrderOrigin.Bling);

                        if (checkResult)
                        {
                            string logMessage = string.Format("Pedido Bling {0} sendo retirado da fila pois ele já foi integrado. Cliente {1}", pedido.Numero, client.Cnpj);
                            Log.Info(logMessage);
                            await discordExtension.AddLogInDiscord(new Adapters.Extensions.Discord.Models.Embed
                            {
                                color = Adapters.Extensions.Discord.Models.DiscordStatusColor.WARNING,
                                title = "<ReceiveOrder> Pedido já foi importado",
                                description = logMessage
                            });
                            return;
                        }

                        ExpeditionOrder order = await _expeditionOrderService.AddOrderAndUpdateInvoice(expeditionOrder);
                        await _expeditionOrderHistoryService.Add(order.Id, "Pedido Gerado - Bling", ExpeditionOrderStatus.Processed, 6);
                        await transaction.CommitAsync();
                    
                    }
                    catch (Exception ex)
                    {
                        string logMessage = string.Format("Pedido Bling {0} sendo retirado da fila pois houve um erro na integração. Cliente {1}. Erro {2}", pedido.Numero, client.Cnpj, ex.Message);
                        Log.Error(logMessage);
                        await discordExtension.AddLogInDiscord(new Adapters.Extensions.Discord.Models.Embed
                        {
                            color = Adapters.Extensions.Discord.Models.DiscordStatusColor.ERROR,
                            title = "<ReceiveOrder> Pedido sendo removido",
                            description = logMessage
                        });
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<ExpeditionOrder> GenerateBlingOrderDtoToModel(BlingPedidoCallbackRequest pedido, Client? client, NfeProc nfeFormated)
        {

            // Create base model

            var model = new ExpeditionOrder()
            {
                ClientName = client.SocialReason,
                Cnpj = client.Cnpj,
                ExternalNumber = pedido.Numero,
                InvoiceAccessKey = pedido.Nota?.ChaveAcesso,
                InvoiceNumber = Convert.ToInt32(pedido.Nota?.Numero),
                InvoiceSerie = Convert.ToInt32(pedido.Nota?.Serie),
                OrderOrigin = OrderOrigin.Bling,
                IssueDate = pedido.Nota?.DataEmissao,
                ShippingCompany = null, //?
                InvoiceValue = Decimal.Parse(pedido.Totalvenda),
                Obs = "Observações: " + (!string.IsNullOrEmpty(pedido.Observacoes) ? pedido.Observacoes : "") + " | Observações Internas: " + (!string.IsNullOrEmpty(pedido.Observacaointerna) ? pedido.Observacaointerna : "") + "",
                Status = ExpeditionOrderStatus.ProcessingPendenting                
            };

            
            model.ShippingMethod = await pedido.TipoIntegracao.GetShippingMethod(client, _integrationRepository);

            if (model.ShippingMethod == ShippingMethodEnum.Kangu)
            {
                model.ShippingMethodCodeOrder = "";

                if (pedido.Transporte != null)
                {
                    if (pedido.Transporte.Volumes != null)
                    {
                        var volume = pedido.Transporte.Volumes.FirstOrDefault();

                        if (volume != null)
                        {
                            model.ShippingMethodCodeOrder = volume.Volume?.CodigoRastreamento;
                        }
                    }
                }
                
            }
            else
            {
                model.ShippingMethodCodeOrder = pedido.NumeroPedidoLoja;
            }
            


            if (model != null)
            {
                var transporte = pedido.Transporte?.EnderecoEntrega;

                if (pedido.Transporte == null || pedido.Transporte.EnderecoEntrega == null)
                {
                    model.Obs += " Pedido não possui dados de transporte";
                    transporte = new BlingEnderecoEntregaCallbackRequest();
                }
                
                var cliente = pedido.Client;

                model.ShippingDetails = new ShippingDetails()
                {
                    Name = transporte.Nome,
                    CpfCnpj = cliente.Cnpj,
                    Rg = cliente.Rg,
                    Address = transporte.Endereco,
                    Number = transporte.Numero,
                    Complement = transporte.Complemento,
                    Neighborhood = transporte.Bairro,
                    Cep = transporte.Cep,
                    City = transporte.Cidade,
                    Uf = transporte.Uf,
                    Phone = client.Phone
                };

                model.ShippingDetailsId = model.ShippingDetails.Id;


                List<ExpeditionOrderItem> orderList = new List<ExpeditionOrderItem>();

                if(nfeFormated.NFe.InfNFe.Det.Count > 0)
                {
                    orderList = nfeFormated.NFe.InfNFe.Det.Select(e => new ExpeditionOrderItem
                    {
                        Name = e.Prod.XProd,
                        Quantity = (decimal)e.Prod.QCom,
                        Description = e.Prod.XProd,
                        ExternalNumberItem = e.Prod.CProd,
                        Value = (decimal)e.Prod.VUnCom,
                        Ean = e.Prod.CEAN,
                        ExpeditionOrderId = model.Id
                    }).ToList();
                }
                else
                {
                    orderList = pedido.Itens.Select(e => new ExpeditionOrderItem
                    {
                        Name = e.Item.Descricao,
                        Quantity = e.Item.Quantidade,
                        Description = e.Item.Descricao,
                        ExternalNumberItem = e.Item.Codigo,
                        Value = e.Item.Valorunidade,
                        Ean = e.Item.Gtin,
                        ExpeditionOrderId = model.Id
                    }).ToList();

                    await ProcessProductsWithoutEan(model);
                    //foreach (var e in pedido.Itens)
                    //{
                    //    ExpeditionOrderItem OrderItem = new ExpeditionOrderItem()
                    //    {
                    //        Name = e.Item.Descricao,
                    //        Quantity = e.Item.Quantidade,
                    //        Description = e.Item.Descricao,
                    //        ExternalNumberItem = e.Item.Codigo,
                    //        Value = e.Item.Valorunidade,
                    //        Ean = e.Item.Gtin,
                    //        ExpeditionOrderId = model.Id
                    //    };
                    //    orderList.Add(OrderItem);
                    //}
                }

                model.ExpeditionOrderItems = orderList;
            }

            //await ProcessProductsWithoutEan(model);

            model.ClientId = client.Id;

            // Try to found product in LOG House System database
            foreach (var item in model.ExpeditionOrderItems)
            {

                /*Product product = new Product()
                {
                    Code = item.ExternalNumberItem,
                    Description = item.Description,
                    Ean = item.Ean,
                    StockQuantity = Convert.ToDouble(item.Quantity),
                    CreatedAt = DateTime.Now,
                    ClientId = client.Id                    
                };

                Product productReturned = _productService.CheckAndAddProduct(product.Ean, product);*/

                Product productReturned = await _productRepository.GetByEanAsync(item.Ean, client.Id);

                if (productReturned != null) { item.ProductId = productReturned.Id; }
            }

            return model;
        }

        public void SaveLog(BlingPedidoCallbackRequest order)
        {
            JObject newClient = JObject.FromObject(order);

            Log.Info($"Novo pedido Bling | Novo pedido recebido no callback de status de pedido da Bling. \n\n Payload: {newClient}");
        }

        public async Task CreateIntegrationVariables(int integrationId)
        {
            var integrationVariables = new IntegrationVariable
            {
                IntegrationId = integrationId,
                Name = BlingIntegrationNames.ApiKey,
                Value = ""
            };

            await _integrationVariableRepository.AddAsync(integrationVariables);
        }

        public async Task<bool> DownloadXmlFile(string invoiceAccess, int clientId)
        {
            await _blingExtentionService.SetClientData(clientId);

            RestResponse response = await _blingExtentionService.GetXmlByAccessKey(invoiceAccess);

            if (response != null)
            {
                await _nFeService.ImportNfeAsync(response.Content, 0);
                return true;
            }

            return false;
        }

        public async Task<(bool, string)> DownloadXmlFileByUrl(string xml, int clientId)
        {
            await _blingExtentionService.SetClientData(clientId);

            RestResponse response = await _blingExtentionService.GetXmlByXml(xml);

            if (response != null)
            {
                return (true, response.Content);
            }

            return (false, null);
        }

        public async Task ProcessProductsWithoutEan(ExpeditionOrder order)
        {

            //Invoice repository
            var invocie = await _invoiceService.GetByAcessKeyAsync(order.InvoiceAccessKey);

            if (invocie != null)
            {
                var someProductWithoutEan = order.ExpeditionOrderItems.Where(e => string.IsNullOrEmpty(e.Ean)).FirstOrDefault();
                
                // if there are some product without ean
                if (someProductWithoutEan != null)
                {
                    order.ExpeditionOrderItems = new List<ExpeditionOrderItem>();

                    foreach (var item in invocie.InvoiceItems)
                    {
                        ExpeditionOrderItem OrderItem = new ExpeditionOrderItem()
                        {
                            Name = item.Description,
                            Quantity = Convert.ToDecimal(item.Quantity),
                            Description = item.Description,
                            ExternalNumberItem = item.Code,
                            Value = item.Value,
                            Ean = item.Ean,
                            ExpeditionOrderId = order.Id
                        };

                        order.ExpeditionOrderItems.Add(OrderItem);
                    }
                }                
            }
        }


        public async Task ProcessBlingOrder(HookInput order)
        {
            string decodedString = HttpUtility.UrlDecode(order.Payload);
            BlingSituacaoPedidoCallbackRequest situacaoPedido = JsonConvert.DeserializeObject<BlingSituacaoPedidoCallbackRequest>(decodedString);
            DiscordExtension discordExtension = new DiscordExtension();

            var client = await _clientsRepository.FindByCnpjAsync(order.Cnpj);

            if (client != null)
            {
                await _blingExtentionService.SetClientData(client.Id);
            }

            if (situacaoPedido.Retorno.Pedidos[0].pedido.Nota == null)
                throw new Exception("Não foi encontrada nenhuma nota para esse pedido");

            var invoiceDataContent = await _blingExtentionService.GetInvoiceData(situacaoPedido.Retorno.Pedidos[0].pedido.Nota.Numero, situacaoPedido.Retorno.Pedidos[0].pedido.Nota.Serie);          

            if (invoiceDataContent.IsSuccessStatusCode)
            {                    
                var invoiceData = JsonConvert.DeserializeObject<BlingInvoiceDataResponseDto>(HttpUtility.UrlDecode(invoiceDataContent.Content));
                invoiceData.Retorno.Notasfiscais = invoiceData.Retorno.Notasfiscais.Where(nf => nf.Notafiscal.NumeroPedidoLoja == situacaoPedido.Retorno.Pedidos[0].pedido.NumeroPedidoLoja).ToArray();

                if (invoiceData.Retorno.Notasfiscais == null || invoiceData.Retorno.Notasfiscais.Length == 0)
                {
                    string message = string.Format("Pedido Bling {0} sendo inativado da fila pois a nota fiscal ainda não está no status de emitida. Cliente {1}", situacaoPedido.Retorno.Pedidos[0].pedido.Numero, order.Cnpj);
                    Log.Info(message);

                    await discordExtension.AddLogInDiscord(new Adapters.Extensions.Discord.Models.Embed
                    {
                        color = Adapters.Extensions.Discord.Models.DiscordStatusColor.ERROR,
                        title = "<ProcessBlingOrder> Hook Pedido Inativado",
                        description = message
                    });

                    try
                    {
                        _retryQueueService.AddHookToRetry(order);

                    }
                    catch (Exception exx)
                    {
                        await discordExtension.AddLogInDiscord(new Adapters.Extensions.Discord.Models.Embed
                        {
                            color = Adapters.Extensions.Discord.Models.DiscordStatusColor.ERROR,
                            title = "<ProcessBlingOrder> Falha ao adicionar para reprocessar",
                            description = $"Err: ```{JObject.FromObject(order)}```  ```{JObject.FromObject(exx)}```"
                        });
                    }
                    throw new Exception(message);
                    //await _hookInputService.InativateHook(order);
                    //return;
                }
                else if (invoiceData.IsInvoiceSended())
                {
                    await ReceiveOrder(situacaoPedido.Retorno.Pedidos[0].pedido, invoiceData.Retorno.Notasfiscais[0].Notafiscal.Xml, order.Cnpj);
                    string logMessage = $"NF processada com sucesso NF {invoiceData.Retorno.Notasfiscais[0].Notafiscal.Numero}";

                    await discordExtension.AddLogInDiscord(new Adapters.Extensions.Discord.Models.Embed
                    {
                        color = Adapters.Extensions.Discord.Models.DiscordStatusColor.SUCCESS,
                        title = "<ProcessBlingOrder> Pedido Processado",
                        description = logMessage
                    });

                    await _hookInputService.DeleteById(order.Id);
                    return;
                }
                else if(invoiceData.IsInvoiceCanceled())
                {
                    string logMessage = string.Format("Pedido Bling NF {0} sendo retirado da fila pois a nota fiscal está cancelada. Cliente {1}", situacaoPedido.Retorno.Pedidos[0].pedido.Numero, order.Cnpj);
                    Log.Info(logMessage);

                    await discordExtension.AddLogInDiscord(new Adapters.Extensions.Discord.Models.Embed
                    {
                        color = Adapters.Extensions.Discord.Models.DiscordStatusColor.WARNING,
                        title = "<ProcessBlingOrder> Hook Pedido deletado",
                        description = logMessage
                    });

                    await _hookInputService.DeleteById(order.Id);
                }
                else
                {
                    string logMessage = string.Format("Pedido Bling {0} NF: sendo inativado da fila pois a nota fiscal ainda não foi enviada. Cliente {1}", situacaoPedido.Retorno.Pedidos[0].pedido.Numero, situacaoPedido.Retorno.Pedidos[0].pedido.Nota.Numero, order.Cnpj);
                    Log.Info(logMessage);

                    await discordExtension.AddLogInDiscord(new Adapters.Extensions.Discord.Models.Embed
                    {
                        color = Adapters.Extensions.Discord.Models.DiscordStatusColor.WARNING,
                        title = "<ProcessBlingOrder> Hook Pedido inativado",
                        description = logMessage
                    });

                    try
                    {
                        _retryQueueService.AddHookToRetry(order);

                    }
                    catch (Exception exx)
                    {
                        await discordExtension.AddLogInDiscord(new Adapters.Extensions.Discord.Models.Embed
                        {
                            color = Adapters.Extensions.Discord.Models.DiscordStatusColor.ERROR,
                            title = "<ProcessBlingOrder> Falha ao adicionar para reprocessar",
                            description = $"Err: order ```{JObject.FromObject(order)}```  ```{JObject.FromObject(exx)}```"
                        });
                    }

                    await _hookInputService.UpdateCode(order, invoiceData.Retorno.Notasfiscais[0].Notafiscal.Numero);

                    throw new Exception(logMessage);
                    //await _hookInputService.InativateHook(order);
                }
            }
           
        }

        public async Task ProcessBlingInvoice(HookInput item)
        {
            string decodedString = HttpUtility.UrlDecode(item.Payload);
            BlingInvoiceDataResponseDto invoiceData = JsonConvert.DeserializeObject<BlingInvoiceDataResponseDto>(decodedString);
            DiscordExtension discordExtension = new DiscordExtension();

            if (invoiceData.Retorno.Notasfiscais == null)
            {
                string logMessage = string.Format("Nota fiscal Bling NF: {0} sendo retirado da fila pois a nota fiscal não foi emitida. Cliente {1}", invoiceData.Retorno.Notasfiscais[0].Notafiscal.Numero, item.Cnpj);
                Log.Info(logMessage);
                
                await _hookInputService.DeleteById(item.Id);

                await discordExtension.AddLogInDiscord(new Adapters.Extensions.Discord.Models.Embed
                {
                    color = Adapters.Extensions.Discord.Models.DiscordStatusColor.ERROR,
                    title = "´<ProcessBlingInvoice> Hook Nota Removido",
                    description = logMessage
                });

                return;
            }
            else if (invoiceData.IsInvoiceSended())
            {
                string logMessage = string.Format("Nota fiscal Bling NF: {0} emitida, ativando Hook de pedido. Cliente {1}", invoiceData.Retorno.Notasfiscais[0].Notafiscal.Numero, item.Cnpj);
                Log.Info(logMessage);
                await discordExtension.AddLogInDiscord(new Adapters.Extensions.Discord.Models.Embed
                {
                    color = Adapters.Extensions.Discord.Models.DiscordStatusColor.WARNING,
                    title = "<ProcessBlingInvoice> Reativar Hook",
                    description = logMessage
                });

                var hook = await _hookInputService.ActivateHookByCodeAndCnpj(invoiceData.Retorno.Notasfiscais[0].Notafiscal.Numero, item.Cnpj);

                if (hook == null)
                {
                    logMessage = string.Format("Nenhum pedido com o numero de NF: {0}. Cliente {1}", invoiceData.Retorno.Notasfiscais[0].Notafiscal.Numero, item.Cnpj);
                    Log.Info(logMessage);

                    await discordExtension.AddLogInDiscord(new Adapters.Extensions.Discord.Models.Embed
                    {
                        color = Adapters.Extensions.Discord.Models.DiscordStatusColor.ERROR,
                        title = "<ProcessBlingInvoice> Hook Pedido não encontrado",
                        description = logMessage
                    });
                }
                else
                {
                    await discordExtension.AddLogInDiscord(new Adapters.Extensions.Discord.Models.Embed
                    {
                        color = Adapters.Extensions.Discord.Models.DiscordStatusColor.SUCCESS,
                        title = "<ProcessBlingInvoice> Hook Reativado",
                        description = logMessage
                    });
                }

                await _hookInputService.DeleteById(item.Id);
                return;
            }
            else if (invoiceData.IsInvoiceCanceled())
            {
                string logMessage = string.Format("Pedido e Nota fiscal Bling NF: {0} sendo retirado da fila pois a nota fiscal está cancelada. Cliente {1}", invoiceData.Retorno.Notasfiscais[0].Notafiscal.Numero, item.Cnpj);
                Log.Info(logMessage);

                await discordExtension.AddLogInDiscord(new Adapters.Extensions.Discord.Models.Embed
                {
                    color = Adapters.Extensions.Discord.Models.DiscordStatusColor.ERROR,
                    title = "<ProcessBlingInvoice> Hook Nota Removido",
                    description = logMessage
                });

                await _hookInputService.DeleteByCodeAndCnpj(invoiceData.Retorno.Notasfiscais[0].Notafiscal.Numero, item.Cnpj);                
                await _hookInputService.DeleteById(item.Id);
            }
            else
            {
                string logMessage = string.Format("Nota fiscal Bling NF: {0} sendo desatiavada da fila pois o status da nota fiscal não foi identificado Status - {1}. Cliente {2}", invoiceData.Retorno.Notasfiscais[0].Notafiscal.Numero, invoiceData.Retorno.Notasfiscais[0].Notafiscal.Situacao, item.Cnpj);
                Log.Info(logMessage);
                await discordExtension.AddLogInDiscord(new Adapters.Extensions.Discord.Models.Embed
                {
                    color = Adapters.Extensions.Discord.Models.DiscordStatusColor.ERROR,
                    title = "<ProcessBlingInvoice> Hook Nota Inativado",
                    description = logMessage
                });

                await _hookInputService.InativateHook(item);
            }
        }
    }
}
