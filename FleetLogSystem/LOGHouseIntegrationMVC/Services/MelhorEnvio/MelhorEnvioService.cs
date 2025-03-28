using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension;
using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension.Dto.Response;
using LOGHouseSystem.Infra.Enum;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using System.Globalization;

namespace LOGHouseSystem.Services
{
    public class MelhorEnvioService : IMelhorEnvioService
    {
        private IMelhorEnvioAPIServices _melhorEnvioAPIService;
        private IExpeditionOrderTagShippingRepository _expeditionOrderShippingRepository;
        private IExpeditionOrderRepository _expeditionOrderRepository;
        private IExpeditionOrderTagShippingService _expeditionOrderTagShippingService;
        private IIntegrationVariableRepository _integrationVariableRepository;

        public MelhorEnvioService(IMelhorEnvioAPIServices melhorEnvioAPIService, 
            IExpeditionOrderTagShippingRepository expeditionOrderShippingRepository, 
            IExpeditionOrderRepository expeditionOrderRepository,
            IExpeditionOrderTagShippingService expeditionOrderTagShippingService,
            IIntegrationVariableRepository integrationVariableRepository)
        {
            _melhorEnvioAPIService = melhorEnvioAPIService;
            _expeditionOrderShippingRepository = expeditionOrderShippingRepository;
            _expeditionOrderRepository = expeditionOrderRepository;
            _expeditionOrderTagShippingService = expeditionOrderTagShippingService;
            _integrationVariableRepository = integrationVariableRepository;
        }

        public async Task GetShippingDataByAccessKey(string order, int clientId, OrderShippingMelhorEnvioDto orderShippiment = null)
        {
            Log.Info($"Processando etiqueta {order} do cliente {clientId}");
            ExpeditionOrderTagShipping orderShipping = await _expeditionOrderShippingRepository.GetShippingByShippingCode(order);

            // Verify if order shipping was previosly inserted
            if (orderShipping == null)
            {
                GetTagMelhorEnvioResponse tag = null;

                // verify if there are order shippiment informatino
                if (orderShippiment == null)
                {
                    // if not, consult tag information
                    tag = await _melhorEnvioAPIService.GetTagInformation(order, clientId); ;
                }

                List<string> orderIds = new List<string>
                {
                    order
                };

                var shipping = await _melhorEnvioAPIService.GetShippingData(orderIds, clientId);

                var completeOrder = await _expeditionOrderRepository.GetOrderByInvoiceAccessKeyAsync(tag.Invoice.InvoiceAccessKey);

                orderShipping = new ExpeditionOrderTagShipping()
                {
                    InvoiceAccessKey = orderShippiment != null ? orderShippiment.Invoice.Key : tag.Invoice.InvoiceAccessKey,
                    OrderTagOrigin = ShippingMethodEnum.MelhorEnvio,
                    ExpeditionOrderId = completeOrder?.Id,
                    ShippingCode = order,
                    Url = shipping.Url
                };

                Log.Info($"Inserindo {order} do cliente {clientId}");
                await _expeditionOrderShippingRepository.AddAsync(orderShipping);
            }
            else
            {
                // if is, update the tag
                Log.Info($"Atualizando {order} do cliente {clientId}");
                await _expeditionOrderTagShippingService.UpdateShippingOrderIntegraded(orderShipping.InvoiceAccessKey, orderShipping);
            }
        }

        public async Task GetAllReleasedShippingData(int clientId)
        {

            Log.Info($"Buscando etiquetas do cliente {clientId}");
            var data = await _melhorEnvioAPIService.GetAllShippingByState(MelhorEnvioOrderState.Released, clientId);

            foreach (var order in data.Data)
            {
                await GetShippingDataByAccessKey(order.Id, clientId, order);
            }
        }

        public async Task DownloadShippment(ExpeditionOrder order)
        {
            if (order.ExpeditionOrderTagShipping != null)
            {
                throw new Exception("Não foi possível baixar a etiqueta, a etiqueta do pedido já foi baixada.") ;
            }

            if (order.ShippingDetails == null)
            {
                throw new Exception("Não foi possível baixar a etiqueta, o pedido não tem os dados de Envio.");
            }

            Log.Info($"Buscando etiquetas do cliente {order.ClientId} pelo nome do cliente {order.ShippingDetails.Name}");

            var data = await _melhorEnvioAPIService.GetAllShippingByName(order.ShippingDetails.Name, Convert.ToInt32(order.ClientId));

            bool founded = false;

            if (data.Data == null)
            {
                throw new Exception($"Não foi possível baixar a etiqueta, nenhuma etiqueta encontrada para o cliente {order.ShippingDetails.Name}.");
            }

            foreach (var item in data.Data)
            {
                if (item.Invoice?.Key == order.InvoiceAccessKey)
                {
                    founded = true;                    
                }
                else if (item.Tags.Count() > 0)
                {
                    foreach (var tag in item.Tags)
                    {
                        if (tag.Tag == order.ShippingMethodCodeOrder)
                        {
                            founded = true;
                            break;
                        }
                    }
                }
                else if (item.Reminder == string.Format($"#{order.ShippingMethodCodeOrder}"))
                {
                    founded = true;
                }
                
                if (founded)
                {
                    await GetShippingDataByOrderId(item, order);
                    break;
                }
            }

            if (!founded)
            {                
                throw new Exception("Etiqueta não encontrada no Melhor envio.");
            }
        }

        public async Task GetShippingDataByOrderId(OrderShippingMelhorEnvioDto orderShippiment, ExpeditionOrder expeditionOrder)
        {
            var clientId = Convert.ToInt32(expeditionOrder.ClientId);

            Log.Info($"Processando etiqueta {orderShippiment.Id} do cliente {clientId}");            

            GetTagMelhorEnvioResponse tag = null;

            List<string> orderIds = new List<string>
            {
                    orderShippiment.Id
            };

            var shipping = await _melhorEnvioAPIService.GetShippingData(orderIds, clientId);

            var completeOrder = expeditionOrder;

            if (completeOrder == null)
            {
                completeOrder = await _expeditionOrderRepository.GetOrderByInvoiceAccessKeyAsync(tag?.Invoice.InvoiceAccessKey);
            }            

            var orderShipping = new ExpeditionOrderTagShipping()
            {
                InvoiceAccessKey = orderShippiment != null ? orderShippiment.Invoice.Key : tag?.Invoice.InvoiceAccessKey,
                OrderTagOrigin = ShippingMethodEnum.MelhorEnvio,
                ExpeditionOrderId = completeOrder?.Id,
                ShippingCode = orderShippiment.Id,
                Url = shipping.Url,
                FileFormat = FileFormatEnum.Url
            };

            Log.Info($"Inserindo {orderShippiment.Id} do cliente {clientId}");
            await _expeditionOrderShippingRepository.AddAsync(orderShipping);
        }

        public List<IntegrationVariable> CreateIntegrationVariables(int integrationId, int clientId)
        {
            List<IntegrationVariable> integrationVariables = new List<IntegrationVariable>();

            string[] integrationVariableNames = new string[] {
                MelhorEnvioIntegrationNames.AccessToken,
                MelhorEnvioIntegrationNames.AccessCreatedAt,
                MelhorEnvioIntegrationNames.RefreshToken,
                MelhorEnvioIntegrationNames.ExpiresIn,
                MelhorEnvioIntegrationNames.Url
            };

            foreach (string variable in integrationVariableNames)
            {

                var value = MelhorEnvioIntegrationNames.Url == variable ?  _melhorEnvioAPIService.GetBaseUrlAuthorizationCode(clientId) : "";

                if (variable == MelhorEnvioIntegrationNames.AccessCreatedAt)
                {
                    //DateTime parsedDateTime = DateTime.ParseExact(value, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    string formattedDateTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

                    integrationVariables.Add(new IntegrationVariable
                    {
                        IntegrationId = integrationId,
                        Name = variable,
                        Value = formattedDateTime
                    });
                }
                else
                {
                    integrationVariables.Add(new IntegrationVariable
                    {
                        IntegrationId = integrationId,
                        Name = variable,
                        Value = value
                    });
                }

            }

            _integrationVariableRepository.AddRange(integrationVariables);

            return integrationVariables;
        }
    }
}
