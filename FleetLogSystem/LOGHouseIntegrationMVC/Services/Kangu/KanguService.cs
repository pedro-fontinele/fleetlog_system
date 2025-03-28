using LOGHouseSystem.Adapters.Extensions.BlingExtension;
using LOGHouseSystem.Adapters.Extensions.Kangu;
using LOGHouseSystem.Adapters.Extensions.Kangu.Dto;
using LOGHouseSystem.Adapters.Extensions.TinyExtension.Service;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.Services.Tiny.Dto;
using Microsoft.AspNetCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace LOGHouseSystem.Services
{
    public class KanguService : IKanguService
    {
        private IIntegrationVariableRepository _integrationVariableRepository;
        private IKanguExtensionService _kanguExtensionService;
        private IClientsRepository _clientsRepository;
        private IIntegrationRepository _integrationRepository;
        private IExpeditionOrderTagShippingRepository _expeditionOrderTagShippingRepository;        
        private IExpeditionOrderRepository _expeditionOrderRepository;
        private IBlingExtensionService _blingExtensionService;
        private ITinyAPIService _tinyAPIService;

        public KanguService(IIntegrationVariableRepository integrationVariableRepository, 
            IKanguExtensionService kanguExtensionService,
            IClientsRepository clientsRepository,
            IIntegrationRepository integrationRepository,
            IExpeditionOrderTagShippingRepository expeditionOrderTagShippingRepository,            
            IExpeditionOrderRepository expeditionOrderRepository,
            IBlingExtensionService blingExtensionService,
            ITinyAPIService tinyAPIService)
        {
            _integrationVariableRepository = integrationVariableRepository;
            _kanguExtensionService = kanguExtensionService;
            _clientsRepository = clientsRepository;
            _integrationRepository = integrationRepository;
            _expeditionOrderTagShippingRepository = expeditionOrderTagShippingRepository;            
            _expeditionOrderRepository = expeditionOrderRepository;
            _blingExtensionService = blingExtensionService;
            _tinyAPIService = tinyAPIService;
        }
        public async Task CreateIntegrationVariables(int integrationId)
        {
            var integrationVariables = new IntegrationVariable
            {
                IntegrationId = integrationId,
                Name = KanguIntegrationNames.ApiKey,
                Value = ""
            };

            await _integrationVariableRepository.AddAsync(integrationVariables);
        }

        public async Task DownloadShippment(ExpeditionOrder order)
        {
            await SetClientData(Convert.ToInt32(order.ClientId));            

            if (string.IsNullOrEmpty(order.ShippingMethodCodeOrder))
            {
                if (order.OrderOrigin == OrderOrigin.Bling)
                {
                    var orderRequest = await _blingExtensionService.GetOrder(order.ExternalNumber, Convert.ToInt32(order.ClientId));

                    var codigoRastreio = orderRequest.Retorno.Pedidos.FirstOrDefault().pedido.Transporte.Volumes.FirstOrDefault()?.Volume?.CodigoRastreamento;

                    if (!string.IsNullOrEmpty(codigoRastreio))
                    {
                        order.ShippingMethodCodeOrder = codigoRastreio;
                        _expeditionOrderRepository.Update(order);
                    }
                    else
                    {
                        throw new Exception("Pedido sem código de rastreio.");
                    }
                }
                else if (order.OrderOrigin == OrderOrigin.Tiny)
                {
                    var client = await _clientsRepository.GetByIdAsync(Convert.ToInt32(order.ClientId));
                    TinyCompleteOrderRequestDto orderTiny = await _tinyAPIService.GetOrder(order.ExternalNumber, client);
                    
                    var codigoRastreio = orderTiny.Retorno.Pedido.CodigoRastreamento;

                    if (!string.IsNullOrEmpty(codigoRastreio))
                    {
                        order.ShippingMethodCodeOrder = codigoRastreio;
                        _expeditionOrderRepository.Update(order);
                    }
                    else
                    {
                        throw new Exception("Pedido sem código de rastreio.");
                    }
                }
                else
                {
                    throw new Exception("Pedido sem código de rastreio.");
                }                
            }

            var tag = new KanguPostTagRequestDto()
            {
                Modelo = "A6",
                Codigo = new List<string>()
                {
                    order.ShippingMethodCodeOrder
                }
            };

            var response = await _kanguExtensionService.PostTag(tag);

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var message = "";
                var errors = JsonConvert.DeserializeObject<List<KanguShippmentErrorDto>>(response.Content);

                foreach (var erro in errors)
                {
                    message = string.Format("{0} | {1}", message, erro.Mensagem);
                }

                Log.Error(message);
                throw new Exception(message);
            }
            else
            {
                var shippment = JsonConvert.DeserializeObject<KanguShippmentResponseDto>(response.Content);

                if (!string.IsNullOrEmpty(shippment.Pdf))
                {

                    byte[] PDFDecoded = Convert.FromBase64String(shippment.Pdf);

                    string path = string.Format("{0}/{1}-{2}-TAG.pdf", Environment.KanguTags, order.Id, order.ShippingMethodCodeOrder);

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    using (BinaryWriter bw = new BinaryWriter(File.Create(path)))
                    {
                        bw.Write(PDFDecoded);                        
                    }                

                    var orderShipping = new ExpeditionOrderTagShipping()
                    {
                        InvoiceAccessKey = order.InvoiceAccessKey,
                        OrderTagOrigin = ShippingMethodEnum.Kangu,
                        ExpeditionOrderId = order.Id,
                        ShippingCode = order.ShippingMethodCodeOrder,
                        Url = path,
                        FileFormat = FileFormatEnum.Pdf
                    };

                    Log.Info($"Inserindo {order.Id} do cliente {order.ClientId}");
                    await _expeditionOrderTagShippingRepository.AddAsync(orderShipping);
                }
                else
                {
                    Log.Error(shippment.Error?.Mensagem);
                    throw new Exception(shippment.Error?.Mensagem);
                }
            }
        }

        public async Task SetClientData(int clientId)
        {
            var client = await _clientsRepository.GetByIdAsync(clientId);

            var integrationsData = await _integrationRepository.GetByClientIdAndNameAsync(clientId, KanguIntegrationNames.IntegrationName);

            if (integrationsData == null)
                throw new Exception($"Dados de conexão do Kangu do cliente {client.SocialReason} não estão cadastrados.");

            var integrationsVariables = await _integrationVariableRepository.GetByIntegrationIdAsync(integrationsData.Id);

            if (integrationsVariables.Count < 1)
                throw new Exception($"Dados de conexão do Kangu do cliente {client.SocialReason} não estão cadastrados.");

            _kanguExtensionService.SetConnection(
                integrationsVariables.Where(e => e.Name == KanguIntegrationNames.ApiKey).FirstOrDefault().Value);
        }
    }
}
