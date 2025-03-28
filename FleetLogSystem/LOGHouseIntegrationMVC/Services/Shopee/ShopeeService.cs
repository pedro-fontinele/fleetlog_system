using DocumentFormat.OpenXml.Drawing.Charts;
using LOGHouseSystem.Adapters.Extensions.ShopeeExtension;
using LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto;
using LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Request;
using LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto.Response;
using LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Enum;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Newtonsoft.Json;
using System.IO.Compression;

namespace LOGHouseSystem.Services
{
    public class ShopeeService : IShopeeService
    {
        private IAPIShopeeService _apiShopeeService;
        private IExpeditionOrderTagShippingRepository _expeditionOrderShippingRepository;
        private IIntegrationVariableRepository _integrationVariableRepository;

        public ShopeeService(IAPIShopeeService apiShopeeService, IExpeditionOrderTagShippingRepository expeditionOrderShippingRepository, IIntegrationVariableRepository integrationVariableRepository)
        {
            _apiShopeeService = apiShopeeService;
            _expeditionOrderShippingRepository = expeditionOrderShippingRepository;
            _integrationVariableRepository = integrationVariableRepository;
        }

        public async Task ProcessAllOrderDocuments(int clientId)
        {
            try
            {
                Log.Info(string.Format("Iniciando processamento de todos os documentos do ClientId: {0}", clientId.ToString()));

                var orders = await _apiShopeeService.GetShipmentList(clientId);

                if (string.IsNullOrEmpty(orders.Error))
                {
                    foreach (var order in orders.Response.OrderList)
                    {
                        await DownloadPdfShippingFile(order, clientId);
                    }
                }
                else
                {
                    var error = string.Format("Erro na busca de Shipment List na Shopee. ClientId: {0} - Message: {1}", clientId.ToString(), orders.Error);
                    Log.Error(error);
                    throw new ShopeeException(error);
                }

                Log.Info(string.Format("Finalizando processamento de todos os documentos do ClientId: {0}", clientId.ToString()));
            }
            catch (UnAuthorizedRequestShopeeException ex)
            {
                Log.Info(string.Format("Requisição não autorizada, notificação realizada. ClientId: {0}", clientId.ToString()));
                throw ex;
            }
            
        }

        public async Task DownloadPdfShippingFile(ShipmentListShiptShopeeDto order, int clientId, ExpeditionOrder expeditionOrder = null)
        {
            try 
            {
                HttpResponseMessage fileDownload = await TryGetDocument(order, clientId);

                Log.Info(string.Format("Gerando ZIP. Pedido: {0}, do ClientId: {0}", order.OrderNumber, clientId.ToString()));

                var fileZipName = await SaveZipFile(order, clientId, fileDownload);

                Log.Info(string.Format("Retirando ZPL do ZIP. Pedido: {0}, do ClientId: {0}", order.OrderNumber, clientId.ToString()));

                string fileName = TakeZplFromZip(order, clientId, fileZipName);

                Log.Info(string.Format("Processo de shipping finalizado com sucesso. Pedido: {0}, do ClientId: {0}", order.OrderNumber, clientId.ToString()));

                var orderShipping = new ExpeditionOrderTagShipping()
                {
                    InvoiceAccessKey = expeditionOrder?.InvoiceAccessKey,
                    OrderTagOrigin = ShippingMethodEnum.Shopee,
                    ExpeditionOrderId = expeditionOrder?.Id,
                    ShippingCode = order.OrderNumber,
                    Url = fileName,
                    FileFormat = FileFormatEnum.Zpl
                };

                Log.Info($"Inserindo {order.OrderNumber} do cliente {clientId}");
                await _expeditionOrderShippingRepository.AddAsync(orderShipping);

            }
            catch (UnAuthorizedRequestShopeeException ex)
            {
                Log.Info(string.Format("Requisição não autorizada, notificação realizada. ClientId: {0}", clientId.ToString()));
                throw ex;
            }
        }

        private string TakeZplFromZip(ShipmentListShiptShopeeDto order, int clientId, string fileZipName)
        {
            var fileName = string.Format("{0}/{1}-{2}.zpl", Environment.ShopeeEnvironment.PdfShipping, order.OrderNumber, clientId);

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (ZipArchive zip = ZipFile.OpenRead(fileZipName))
            {
                foreach (ZipArchiveEntry entry in zip.Entries.Where(e => e.FullName.Contains("thermal_zpl_shipping_label.txt")))
                {
                    entry.ExtractToFile(fileName);
                }
            }

            File.Delete(fileZipName);

            return fileName;
        }

        private async Task<string> SaveZipFile(ShipmentListShiptShopeeDto order, int clientId, HttpResponseMessage fileDownload)
        {
            var fileZipName = string.Format("{0}/{1}-{2}-ZIP.zip", Environment.ShopeeEnvironment.PdfShipping, order.OrderNumber, clientId);

            var contentBytes = await fileDownload.Content.ReadAsByteArrayAsync();

            if (File.Exists(fileZipName))
            {
                File.Delete(fileZipName);
            }

            // Save this file
            File.WriteAllBytes(fileZipName, contentBytes);

            return fileZipName;
        }

        private async Task<HttpResponseMessage> TryGetDocument(ShipmentListShiptShopeeDto order, int clientId)
        {
            var formatDocument = DocumentTypeEnum.ThermalAirWaybill;

            var fileDownload = await CallDownloadFile(order, clientId, formatDocument);

            var hasError = await ValidateErrorShopee(fileDownload);

            if (hasError)
            {
                fileDownload = await TryGenerateDocument(order, clientId, formatDocument);
            }

            return fileDownload;
        }

        private async Task<HttpResponseMessage> TryGenerateDocument(ShipmentListShiptShopeeDto order, int clientId, string formatDocument)
        {
            await VerifyStatusOrder(order, clientId, formatDocument);

            HttpResponseMessage fileDownload = await CallDownloadFile(order, clientId, formatDocument);
            await ValidateErrorShopee(fileDownload, true);
            

            return fileDownload;
        }

        private async Task VerifyStatusOrder(ShipmentListShiptShopeeDto order, int clientId, string documentFormat)
        {
            var orderDetailsReponse = await _apiShopeeService.GetShippingDetails(order.OrderNumber, clientId);

            var text = await orderDetailsReponse.Content.ReadAsStringAsync();

            var statusOrders = JsonConvert.DeserializeObject<ShippingDetailsShopeeDto>(text);
            

            if (!string.IsNullOrEmpty(statusOrders.message))
                TriggerTagException(statusOrders.message);


            if (statusOrders.response.order_list.Count <= 0)
                TriggerTagException("Nenhum pedido encontrado");

            var orderShopee = statusOrders.response.order_list[0];

            if (orderShopee.order_status == ShopeeOrderStatusEnum.Shipped)
                TriggerTagException("O pedido já foi entregue");

            if (orderShopee.order_status == ShopeeOrderStatusEnum.ReadyToShip)
            {
                var responseParameters = await _apiShopeeService.GetShippingParamterAsync(order.OrderNumber, clientId);

                text = await responseParameters.Content.ReadAsStringAsync();

                var parametersResponse = JsonConvert.DeserializeObject<GetShippingParamtersShopeeResponse>(text);

                ShipOrderShopeeRequest data = GenerateShipOrderRequestDto(parametersResponse, order.OrderNumber);                

                var response = await _apiShopeeService.ShipOrderAsync(data, clientId);

                await ValidateErrorShopee(response, true);
            }

            string trackingNumber = await TryGetTrackingNumber(order.OrderNumber, clientId);

            await CreateShippingDocument(order, clientId, trackingNumber, documentFormat);
        }

        private ShipOrderShopeeRequest GenerateShipOrderRequestDto(GetShippingParamtersShopeeResponse parametersResponse, string order_sm)
        {
            var data = new ShipOrderShopeeRequest()
            {
                order_sn = order_sm
            };

            data.dropoff = GenerateShipOrderDropoff(parametersResponse, data);
            data.pickup = GenerateShipOrderPickup(parametersResponse, data);

            return data;
        }

        private async Task<string> TryGetTrackingNumber(string orderNumber, int clientId)
        {
            var response = await _apiShopeeService.GetTrackingNumberAsync(orderNumber, clientId);
            await ValidateErrorShopee(response, true);

            var text = await response.Content.ReadAsStringAsync();

            var trackerResponse = JsonConvert.DeserializeObject<GetTrackingNumberResponseShopeeDto>(text);

            return trackerResponse.response.tracking_number;
        }

        private async Task CreateShippingDocument(ShipmentListShiptShopeeDto order, int clientId, string trackingNumber,  string documentFormat)
        {
            var orderListGeneration = ConvertShipmentListShiptShopeeDtoToCreateShippingDocumentShopeeRequestDto(order, trackingNumber, documentFormat);

            var response = await _apiShopeeService.GeneratingShippingDocumentAsync(orderListGeneration, clientId);

            var hasErros = await ValidateErrorShopee(response);

            if (!hasErros)
            {
                var downloadReady = false;

                var timesTrying = 1;

                while(!downloadReady || timesTrying <= 3)
                {
                    Thread.Sleep(5000);
                    var result = await _apiShopeeService.GetShippingDocumentResultAsync(orderListGeneration, clientId);

                    await ValidateErrorShopee(result, true);

                    var text = await result.Content.ReadAsStringAsync();

                    var getResult = JsonConvert.DeserializeObject<GetShippingResultShopeeResponseDto>(text);

                    if (getResult.response.result_list[0].status == ShopeeOrderStatusEnum.Ready)
                    {
                        downloadReady = true;
                        break;
                    }

                    timesTrying++;
                }

                if (!downloadReady)
                {
                    TriggerTagException("Etiqueta não ficou pronta em tempo habil");
                }
                
            }
        }

        private void TriggerTagException(string message)
        {
            throw new Exception(string.Format("Não foi possível realizar a busca do pedido - {0}", message));
        }

        private async Task<HttpResponseMessage> CallDownloadFile(ShipmentListShiptShopeeDto order, int clientId, string documentFormat)
        {
            Log.Info(string.Format("Baixando etiquetas de shipping Pedido: {0}, do ClientId: {0}", order.OrderNumber, clientId.ToString()));
            var documentResponse = await _apiShopeeService.DownloadShippingDocumentAsync(new List<ShipmentListShiptShopeeDto>()
                        {
                            order
                        }, documentFormat, clientId);

            Log.Info(string.Format("Consulta de shipping realizada com sucesso do Pedido: {0},  do ClientId: {0}", order.OrderNumber, clientId.ToString()));

            return documentResponse;
        }

        private async Task<bool> ValidateErrorShopee(HttpResponseMessage response, bool triggerException = false)
        {
            var text = await response.Content.ReadAsStringAsync();
            bool hasErrors = false;
            try
            {                
                var responseJson = JsonConvert.DeserializeObject<ErrorShopeeDto>(text);
                
                if (!string.IsNullOrEmpty(responseJson.Error))
                {
                    string message = string.Format("Erro ao tentar fazer o download da etiqueta da Shopee: {0}", responseJson.Message);

                    message = AddErrorMessageResultListResponse(responseJson, message);
                    Log.Info(message);

                    if (triggerException)
                    {
                        TriggerTagException(message);
                    }
                    hasErrors = true;
                }

            }
            catch(Exception ex)
            {
                Log.Info(string.Format("Nenhum erro pré detectado encontrado, provavelmente o texto é uma etiqueta Texto: {0} - {1} - {2} - {3}", text, ex.Message, ex.InnerException, ex.StackTrace));
            }            

            return hasErrors;
        }

        private CreateShippingDocumentShopeeRequestDto ConvertShipmentListShiptShopeeDtoToCreateShippingDocumentShopeeRequestDto(ShipmentListShiptShopeeDto order, string trackingNumber, string documentFormat)
        {
            return new CreateShippingDocumentShopeeRequestDto()
            {
                OrderList = new List<CreateShipmentListShiptShopeeDto>()
                    {
                        new CreateShipmentListShiptShopeeDto()
                        {
                            OrderNumber = order.OrderNumber,
                            PackageNumber = order.PackageNumber,
                            TrackingNumber = trackingNumber,
                            ShippingDocumentType = documentFormat
                        }
                    }
            };
        }



        private ShipOrderPickupShopeeRequest GenerateShipOrderPickup(GetShippingParamtersShopeeResponse parametersResponse, ShipOrderShopeeRequest data)
        {
            if (parametersResponse.response.info_needed.pickup != null)
            {
                data.pickup = new ShipOrderPickupShopeeRequest();

                var exists = parametersResponse.response.info_needed.pickup.Any(e => e == "address_id");

                if (exists)
                {
                    var address = parametersResponse.response.pickup.address_list.FirstOrDefault();

                    if (address != null)
                    {
                        data.pickup.address_id = address.address_id;
                    }
                }

                exists = parametersResponse.response.info_needed.pickup.Any(e => e == "pickup_time_id");

                if (exists)
                {
                    var timeSlot = parametersResponse.response.pickup.time_slot_list.FirstOrDefault();

                    if (timeSlot != null)
                    {
                        data.pickup.pickup_time_id = timeSlot.pickup_time_id;
                    }
                }
            }

            return data.pickup;
        }

        private ShipOrderDropoffShopeeRequest GenerateShipOrderDropoff(GetShippingParamtersShopeeResponse parametersResponse, ShipOrderShopeeRequest data)
        {
            if (parametersResponse.response.info_needed.dropoff != null)
            {
                data.dropoff = new ShipOrderDropoffShopeeRequest();

                var exists = parametersResponse.response.info_needed.dropoff.Any(e => e == "branch_id");

                if (exists)
                {
                    var branch = parametersResponse.response.dropoff.branch_list.FirstOrDefault();

                    if (branch != null)
                    {
                        data.dropoff.branch_id = branch.branch_id;
                    }
                }

                exists = parametersResponse.response.info_needed.dropoff.Any(e => e == "slug");

                if (exists)
                {
                    var slug = parametersResponse.response.dropoff.slug_list.FirstOrDefault();

                    if (slug != null)
                    {
                        data.dropoff.slug = slug.slug;
                    }
                }
            }

            return data.dropoff;
        }

        private string AddErrorMessageResultListResponse(ErrorShopeeDto responseJson, string message)
        {
            if (responseJson.Response != null && responseJson.Response.ResultList != null && responseJson.Response.ResultList.Count > 0)
            {
                var formatedResponses = responseJson.Response.ResultList.Select(e => string.Format("{0}: {1}", e.Order, e.FailError));
                var stringConcat = string.Join(",", formatedResponses);

                if (message.Length > 0){
                    message = message + " " + stringConcat;
                }
                else{
                    message = stringConcat;
                }                
            }

            return message;
        }        

        public async Task DownloadPdfShippingFileByExpeditionOrder(ExpeditionOrder order)
        {
            var shopeeNumer = order.ShippingMethodCodeOrder;

            await DownloadPdfShippingFile(new ShipmentListShiptShopeeDto()
            {
                OrderNumber = shopeeNumer
            }, Convert.ToInt32(order.ClientId), order);
        }


        public void CreateIntegrationVariables(int integrationId, int clientId)
        {
            List<IntegrationVariable> integrationVariables = new List<IntegrationVariable>();

            string[] integrationVariableNames = new string[] {
                ShopeeIntegrationNames.AccessToken,
                ShopeeIntegrationNames.AccessCreatedAt,
                ShopeeIntegrationNames.RefreshToken,
                ShopeeIntegrationNames.ShopId,               
                ShopeeIntegrationNames.Url
            };

            foreach (string variable in integrationVariableNames)
            {

                var value = ShopeeIntegrationNames.Url == variable ? _apiShopeeService.GenerateAuthorizationCodeUrlSave(clientId) : "";

                integrationVariables.Add(new IntegrationVariable
                {
                    IntegrationId = integrationId,
                    Name = variable,
                    Value = value
                });
            }

            _integrationVariableRepository.AddRange(integrationVariables);
        }
    }
}
