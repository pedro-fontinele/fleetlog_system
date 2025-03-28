using LOGHouseSystem.Adapters.Extensions.BlingExtension;
using LOGHouseSystem.Adapters.Extensions.BlingExtension.Dto.Hook.Request;
using LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension;
using LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Newtonsoft.Json;

namespace LOGHouseSystem.Services
{
    public class BlingService : IBlingService
    {
        private IAPIBlingService _iAPIBlingService;
        private IBlingCallbackService _blingCallbackService;
        private IClientsRepository _clientsRepository;
        private IBlingExtensionService _blingExtensionService;
        private IHookInputService _hookInputService;

        public BlingService(IAPIBlingService iAPIBlingService, IBlingCallbackService blingCallbackService, IClientsRepository clientsRepository, IBlingExtensionService blingExtensionService, IHookInputService hookInputService)
        {
            _iAPIBlingService = iAPIBlingService;
            _blingCallbackService = blingCallbackService;
            _clientsRepository = clientsRepository;
            _blingExtensionService = blingExtensionService;
            _hookInputService = hookInputService;
        }

        public async Task IntegrateOrders(OrderIntegrationViewModel orderSearch)
        {
            if (orderSearch.EntryDateStart == null || orderSearch.EntryDateStart == new DateTime(1, 1, 1, 0, 0, 0))
            {
                throw new Exception("É necessário informar uma data de inicio");
            }

            if (orderSearch.EntryDateEnd == null || orderSearch.EntryDateEnd == new DateTime(1, 1, 1, 0, 0, 0))
            {
                throw new Exception("É necessário informar uma data final");
            }

            if (orderSearch.ClientId == 0 || orderSearch.ClientId == null)
            {
                throw new Exception("É necessário informar um cliente");
            }

            if (orderSearch.EntryDateStart > orderSearch.EntryDateEnd)
            {
                throw new Exception("A data inicial deve ser menor que a data final");
            }

            if (orderSearch.EntryDateEnd > Convert.ToDateTime(orderSearch.EntryDateStart).AddDays(7))
            {
                throw new Exception("O intervalo de datas não deve ser maior que 7 dias para um cliente");
            }

            await IntegrateOrdersV2(orderSearch);
        }

        private async Task IntegrateOrdersV2(OrderIntegrationViewModel orderSearch)
        {
            int page = 1;
            bool continueLoop = true;
            List<BlingPedidoListCallbackRequest> orderToProcess = new List<BlingPedidoListCallbackRequest>();

            do
            {
                var ordersResponse = await _blingExtensionService.GetOrders(Convert.ToDateTime(orderSearch.EntryDateStart), Convert.ToDateTime(orderSearch.EntryDateEnd), Convert.ToInt32(orderSearch.ClientId),page);

                var orders = JsonConvert.DeserializeObject<BlingSituacaoPedidoCallbackRequest>(ordersResponse.Content);

                if (orders.Retorno.Erros?.Count > 0)
                {
                    continueLoop = false;
                    //throw new Exception(string.Join(", ", orders.Retorno.Erros.Select(e => e.Erro.Msg)));
                }
                else
                {
                    orderToProcess.AddRange(orders.Retorno.Pedidos);
                }

                page++;
            } while (continueLoop);


            List<string> messageErrors = new List<string>();

            orderSearch.Orders = new List<BlingNotaCallbackRequest>();

            foreach (var item in orderToProcess)
            {
                try
                {
                    var newItem = new BlingSituacaoPedidoCallbackRequest()
                    {
                        Retorno = new BlingRetornoCallbackRequest()
                        {
                            Pedidos = new List<BlingPedidoListCallbackRequest>()
                            {
                                item
                            }
                        }
                    };

                    orderSearch.Orders.Add(item.pedido.Nota);

                    var client = await _clientsRepository.GetByIdAsync(Convert.ToInt32(orderSearch.ClientId));

                    

                    await _blingCallbackService.ProcessBlingOrder(new HookInput()
                    {
                        Cnpj = client.Cnpj,
                        Status = false,
                        Type = HookTypeEnum.Order,
                        Payload = JsonConvert.SerializeObject(newItem),
                        Origin = OrderOrigin.Bling
                    });

                    //await hookInputService.Add(new HookInput()
                    //{
                    //    Cnpj = client.Cnpj,
                    //    Status = true,
                    //    Type = HookTypeEnum.Order,
                    //    Payload = JsonConvert.SerializeObject(newItem),
                    //    Origin = OrderOrigin.Bling
                    //});

                }
                catch (Exception ex)
                {
                    messageErrors.Add($"Houve um erro no porcessamento do pedido {item.pedido.Numero} confira o LOG para mais informações");
                    Log.Error(string.Format("{0} - {1}", ex.Message, ex.StackTrace));
                }
            }
        }

        private async Task IntegrateOrdersV3(OrderIntegrationViewModel orderSearch)
        {
            var ordersResponse = await _iAPIBlingService.GetOrders(Convert.ToDateTime(orderSearch.EntryDateStart), Convert.ToDateTime(orderSearch.EntryDateEnd), Convert.ToInt32(orderSearch.ClientId));

            if (ordersResponse.IsSuccessStatusCode)
            {
                var orders = JsonConvert.DeserializeObject<OrdersSearchBlingV3>(ordersResponse.Content);

                if (orders.Data.Count <= 0)
                {
                    throw new Exception($"Nenhum pedido foi encontrado");
                }

                List<string> messageErrors = new List<string>();

                foreach (var item in orders.Data)
                {
                    try
                    {
                        var orderCompleteResponse = await _iAPIBlingService.GetOrder(item.Id.ToString(), Convert.ToInt32(orderSearch.ClientId));

                        if (orderCompleteResponse.IsSuccessStatusCode)
                        {
                            OrderBlingV3 orderComplete = JsonConvert.DeserializeObject<OrderBlingV3>(orderCompleteResponse.Content);

                            var map = OrderV3MappingExtensions.MapOrderBlingV3ToV2(orderComplete);

                            var invoiceSearch = await _iAPIBlingService.GetNotesBySearch(orderComplete.numeroLoja, Convert.ToInt32(orderSearch.ClientId));

                            BlingV3InvoiceResponseDto invoice = JsonConvert.DeserializeObject<BlingV3InvoiceResponseDto>(invoiceSearch.Content);

                            var invoiceItem = invoice?.data.Count > 0 ? invoice?.data[0] : null;

                            if (invoiceItem == null)
                            {
                                throw new Exception($"Nenhuma nota encontrada para o pedido {item.Id}");
                            }

                            var invoiceCompletaResponse = await _iAPIBlingService.GetNoteByInvoiceId(invoiceItem.id.ToString(), Convert.ToInt32(orderSearch.ClientId));
                            BlingV3CompleteInvoiceResponseDto invoiceCompleta = JsonConvert.DeserializeObject<BlingV3CompleteInvoiceResponseDto>(invoiceCompletaResponse.Content);

                            var client = await _clientsRepository.GetByIdAsync(Convert.ToInt32(orderSearch.ClientId));

                            await _blingCallbackService.ReceiveOrder(map, invoiceCompleta.data.xml, client.Cnpj);
                        }
                    }
                    catch (Exception ex)
                    {
                        messageErrors.Add($"Houve um erro no porcessamento do pedido {item.Id} confira o LOG para mais informações");
                        Log.Error(string.Format("{0} - {1}", ex.Message, ex.StackTrace));
                    }
                }

                if (messageErrors.Count > 0)
                {
                    throw new Exception(string.Join(", ", messageErrors));
                }
            }
            else
            {
                throw new Exception($"Houve um erro na requisição: {ordersResponse.Content}");
            }
        }
    }
}
