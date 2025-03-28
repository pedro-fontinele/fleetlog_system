using DocumentFormat.OpenXml.Drawing.Charts;
using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request;
using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Response;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using PagedList;

namespace LOGHouseSystem.Services
{
    public class PackingListTransportationService : IPackingListTransportationService
    {
        private IPackingListTransportationRepository _packingListTransportationRepository;
        private IExpeditionOrderService _expeditionOrderService;
        private IExpeditionOrderHistoryService _expeditionOrderHistoryService;
        private IPackingListTransportationHistoryRepository _packingListTransportationHistoryRepository;
        private IUserRepository _userRepository;

        public PackingListTransportationService(IPackingListTransportationRepository packingListTransportationRepository,
            IExpeditionOrderService expeditionOrderService,
            IExpeditionOrderHistoryService expeditionOrderHistoryService,
            IPackingListTransportationHistoryRepository packingListTransportationHistoryRepository,
            IUserRepository userRepository)
        {
            _packingListTransportationRepository = packingListTransportationRepository;
            _expeditionOrderService = expeditionOrderService;
            _expeditionOrderHistoryService = expeditionOrderHistoryService;
            _packingListTransportationHistoryRepository = packingListTransportationHistoryRepository;
            _userRepository = userRepository;
        }

        public PagedList<PackingListTransportation> GetAllPaged(int Page)
        {
            int PageSize = 10;

            return _packingListTransportationRepository.GetAllPaged(Page, PageSize);
        }

        public async Task<PackingListTransportationResponse> Add(PackingListTransportationRequest request, int? userId = null)
        {

            PackingListTransportation packingListTransportation = new PackingListTransportation()
            {
                CreatedAt = DateTimeHelper.GetCurrentDateTime(),
                TransportationPersonId = request.TransportationPersonId,
                ShippingCompanyId = request.ShippingCompanyId,
                VehiclePlate = request.VehiclePlate,
                Observation = request.Observation,
            };

            var packingAdded = await _packingListTransportationRepository.AddAsync(packingListTransportation);

            if(packingAdded != null)
            {
                PackingListTransportationResponse responseDto = new PackingListTransportationResponse()
                {
                    Id = packingAdded.Id,
                    CreatedAt = packingAdded.CreatedAt,
                    TransportationPersonId = packingAdded.TransportationPersonId,
                    ShippingCompanyId = packingAdded.ShippingCompanyId,
                    VehiclePlate = packingAdded.VehiclePlate,
                    Observation = packingAdded.Observation,
                };

                var userLogedId = userId ?? _userRepository.GetUserLoged().Id;

                var packingTransportationHistory = new PackingListTransportationHistory()
                {
                    Date = DateTime.Now,
                    Status = PackingListTransportationStatus.Gerado,
                    PackingListTransportationId = packingAdded.Id,
                    UserId = userLogedId
                };

                await _packingListTransportationHistoryRepository.AddAsync(packingTransportationHistory);


                return responseDto;
            }

            throw new ArgumentNullException("Não foi possível adicionar o romaneio, tente novamente!");
        }

        public async Task SignTransportation(int id, List<IFormFile> signImage, int userId)
        {
            var packinListTransportation = _packingListTransportationRepository.GetById(id);

            if (packinListTransportation == null)
            {
                throw new Exception("Nenhum romaneio foi encontrado com esse Id");
            }

            if (signImage == null || signImage.Count == 0)
            {
                throw new Exception("Assinatura obrigatória para finalização do romaneio");
            }

            if (signImage.Count > 0)
            {
                var file = signImage[0];

                if (file.Length > 0)
                {
                    string[] allowedFormats = { ".jpg", ".jpeg", ".png", ".gif" };
                    string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (!allowedFormats.Contains(fileExtension))
                    {
                        throw new Exception("O arquivo enviado não é uma imagem válida.");
                    }

                    // new file name
                    var filePath = $"{Environment.TransportationSignaturePath}/{id}" + fileExtension;

                    // Delete image if exists
                    if (File.Exists(filePath))
                    {
                        // Excluir o arquivo existente
                        File.Delete(filePath);
                    }

                    // save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    // removing wwwroot path because is not part of the route file
                    packinListTransportation.SignatureImgPath = filePath.Replace("wwwroot/", "~/");
                }
            }
            packinListTransportation.Status = Infra.Enums.PackingListTransportationStatus.Finalizado;

            await _packingListTransportationRepository.UpdateAsync(packinListTransportation);

            var userLogedId = userId;

            var packingTransportationHistory = new PackingListTransportationHistory()
            {
                Date = DateTime.Now,
                Status = PackingListTransportationStatus.Finalizado,
                PackingListTransportationId = packinListTransportation.Id,
                UserId = userLogedId,
            };

            await _packingListTransportationHistoryRepository.AddAsync(packingTransportationHistory);

            foreach (var order in packinListTransportation.ExpeditionOrders)
            {
                var finalizeDate = DateTimeHelper.GetCurrentDateTime();
                await _expeditionOrderService.UpdateOrder(order.Id, ExpeditionOrderStatus.Dispatched, finalizeDate);
                await _expeditionOrderHistoryService.Add(order.Id, "Pedido Despachado", ExpeditionOrderStatus.Dispatched, userId);
            }


        }

        public async Task SaveTransportPlate(int id, List<IFormFile> plateImage)
        {
            var packinListTransportation = _packingListTransportationRepository.GetById(id);

            if (packinListTransportation == null)
            {
                throw new Exception("Nenhum romaneio foi encontrado com esse Id");
            }

            if (plateImage == null || plateImage.Count == 0)
            {
                throw new Exception("A foto da placa não foi enviada");
            }

            if (plateImage.Count > 0)
            {
                var file = plateImage[0];

                if (file.Length > 0)
                {
                    string[] allowedFormats = { ".jpg", ".jpeg", ".png", ".gif" };
                    string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (!allowedFormats.Contains(fileExtension))
                    {
                        throw new Exception("O arquivo enviado não é uma imagem válida.");
                    }

                    // new file name
                    var filePath = $"{Environment.TransportationPlatesPath}/{id}" + fileExtension;

                    // Delete image if exists
                    if (File.Exists(filePath))
                    {
                        // Excluir o arquivo existente
                        File.Delete(filePath);
                    }

                    // save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                }
            }
            
        }

        public PagedList<PackingListTransportation> GetByFilter(FilterPackingListTransportationViewModel filter)
        {
            return _packingListTransportationRepository.GetByFilters(filter);
        }

        
    }
}
