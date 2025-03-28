using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using PagedList;

namespace LOGHouseSystem.Services
{
    public class PackingService : IPackingService
    {
        private readonly IPackingRepository _packingRepository;
        private readonly IPackingItemService _packingItemService;
        private readonly IExpeditionOrderRepository _expeditionOrderRepository;
        private readonly IPackingHistoryService _packingHistoryService;
        private readonly IPickingListRepository _pickingListRepository;

        public PackingService(IPackingRepository packingRepository,
                              IExpeditionOrderRepository expeditionOrderRepository,
                              IPackingItemService packingItemService,
                              IPackingHistoryService packingHistoryService,
                              IPickingListRepository pickingListRepository)
        {
            _packingRepository = packingRepository;
            _expeditionOrderRepository = expeditionOrderRepository;
            _packingItemService = packingItemService;
            _packingHistoryService = packingHistoryService;
            _pickingListRepository = pickingListRepository;
        }

        public List<Packing> GetWithStatusEmAtendimentoAndGerado()
        {
            return _packingRepository.GetWithStatusGeradoOrEmAtendimento();
        }

        public PagedList<Packing> GetAllPaged(int Page)
        {
            int pageSize = 10;

            return _packingRepository.GetAllPaged(Page,pageSize);
        }

        public async Task<bool> DeleteById(int id)
        {
           Packing packing = _packingRepository.GetById(id);

            if (packing == null)
                throw new Exception("O empacotamento que você está tentando apagar não foi encontrado, por favor, tente novamente!");

            var deleteReturn =await  _packingRepository.Delete(packing);

            if (deleteReturn != true)
                throw new Exception("Não foi possível deletar esse empacotamento, por favor, tente novamente!");

            return true;
        }

        public Packing AddByOrderExpedition(int id, int userId)
        {
            ExpeditionOrder orderById = _expeditionOrderRepository.GetById(id);
            

            if (orderById == null)
                throw new Exception("Não foi possivel empacotar esse pedido, por favor, tente novamente!");

            Packing newPacking = new Packing
            {
                Responsible = orderById.ClientName,
                Description = "Em empacotamento",
                Quantity = orderById.ExpeditionOrderItems.Count,
                Observation = orderById.Obs,
                Status = PackingStatus.Gerado,
                CreatedAt = DateTime.Now,
                ClientId = orderById.ClientId,
                ExpeditionOrderId = orderById.Id,
            };

            List<PackingItem> items = _packingItemService.MapPakingItemsByExpeditionOrderItems(orderById.ExpeditionOrderItems);
            newPacking.Items = items;

            Packing packing = _packingRepository.Add(newPacking);

            orderById.Status = ExpeditionOrderStatus.InPacking;
            _expeditionOrderRepository.Update(orderById);

            _packingHistoryService.Add(packing.Id, "", PackingStatus.Gerado, userId);

            return newPacking;

        }

        public async Task SendPackingImage(int id, List<IFormFile> formFile, string imageBase64 = "")
        {
            var packing = await _packingRepository.GetByIdAsync(id);

            string filePath = "";

            if (string.IsNullOrEmpty(imageBase64))
            {
                if (formFile.Count <= 0)
                {
                    throw new Exception("É necessário ao menos um arquivo");
                }

                var file = formFile[0];

                if (file.Length < 0)
                {
                    throw new Exception("É necessário ao menos um arquivo");
                }

                string[] allowedFormats = { ".jpg", ".jpeg", ".png", ".gif" };
                string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedFormats.Contains(fileExtension))
                {
                    throw new Exception("O arquivo enviado não é uma imagem válida.");
                }
                

                if (packing == null)
                {
                    throw new Exception("Nenhum pacote foi encontrado com esse Id");
                }

                // new file name
                filePath = $"{Environment.PackingPhotoPath}/{id}" + fileExtension;

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
                packing.ImagePath = filePath.Replace("wwwroot/", "~/");

                await _packingRepository.UpdateAsync(packing);
            }
            else
            {
                imageBase64 = imageBase64.Replace("data:image/png;base64,", "");

                byte[] bytesImagem = Convert.FromBase64String(imageBase64);
                // new file name
                filePath = $"{Environment.PackingPhotoPath}/{id}.png";

                // Delete image if exists
                if (File.Exists(filePath))
                {
                    // Excluir o arquivo existente
                    File.Delete(filePath);
                }

                File.WriteAllBytes(filePath, bytesImagem);
            }

            // removing wwwroot path because is not part of the route file
            packing.ImagePath = filePath.Replace("wwwroot/", "~/");

            await _packingRepository.UpdateAsync(packing);
        }

        public Packing SearchByAccessKey(string invoiceAccessKey)
        {
            if (string.IsNullOrEmpty(invoiceAccessKey)) return null;
            ExpeditionOrder expeditionOrder = _expeditionOrderRepository.GetByInvoiceAccessKey(invoiceAccessKey);
            if(expeditionOrder == null) return null;

            Packing packing = _packingRepository.GetByExpeditionOrderId(expeditionOrder.Id);

            return packing;
        }

        public async Task<Packing> SearchByAccessKeyAsync(string invoiceAccessKey)
        {
            if (string.IsNullOrEmpty(invoiceAccessKey)) return null;
            ExpeditionOrder expeditionOrder = await _expeditionOrderRepository.GetOrderByInvoiceAccessKeyAsync(invoiceAccessKey);
            Packing packing = await _packingRepository.GetByExpeditionOrderIdAsync(expeditionOrder.Id);


            return packing;
        }

        public List<Packing> GeneratePackingByPickingId(int id, int userId)
        {
            //BUSCAR SOH ID necessario para geracao
            List<int> ordersIds = _expeditionOrderRepository.GetOrdersIdsByPickingList(id);

            List<Packing> packingList = new List<Packing>();

            foreach (int orderId in ordersIds)
            {
                Packing packing = AddByOrderExpedition(orderId, userId);
                packingList.Add(packing);
            }

            return packingList;
        }

        public List<Packing> AddByPickingLists(string[] selecteds)
        {

            List<PickingList> pickingLists = new List<PickingList>();

            foreach (var e in selecteds)
            {
                PickingList list = _pickingListRepository.GetById(Convert.ToInt32(e));
                pickingLists.Add(list);
            }

            List<int> ids = new List<int>();

            foreach (var e in pickingLists)
            {
                ids.AddRange(e.ExpeditionOrder.Select(e => e.Id).ToList());
            }

            pickingLists = new List<PickingList>();

            List<Packing> packings = new List<Packing>();

            foreach (int id in ids)
            {
                var exists = _packingRepository.GetByExpeditionOrderId(id);

                if (exists != null)
                {
                    continue;
                }

                Packing packing = AddByOrderExpedition(id, 0);
                packings.Add(packing);
            }

            return packings;
        }

        public PagedList<Packing> GetPackingWhithoutPackingListTrasportationByFilter(FilterPackingWhithoutPackingListTrasportationViewModel filter)
        {
            return _packingRepository.GetPackingWhithoutPackingListTrasportationByFilter(filter);

        }

        public PagedList<Packing> GetByFilter(FilterPackingViewModel filter)
        {
            return _packingRepository.GetByFilters(filter);

        }


    }
}
