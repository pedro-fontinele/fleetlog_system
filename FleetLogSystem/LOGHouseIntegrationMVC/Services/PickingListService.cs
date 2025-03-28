using LOGHouseSystem.Adapters.Extensions.Labelary;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using LOGHouseSystem.Services.Helper;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Services.Interfaces;
using PagedList;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.ViewModels;
using LOGHouseSystem.Adapters.Extensions.NFeExtension;
using System.Collections.Generic;
using System.Net;

namespace LOGHouseSystem.Services
{
    public class PickingListService : IPickingListService
    {
        private readonly IPickingListRepository _pickingListRepository;
        private readonly IPackingRepository _packingRepository;
        private readonly IPickingListItemRepository _pickingListItemRepository;
        private readonly ILabelaryAPIService _labelaryAPIService;
        private readonly IReceiptNoteRepository _receiptNoteRepository;
        private readonly ISimplifiedDanfeService _simplifiedDanfeService;
        private IInvoiceService _invoiceService;
        private IZplToPdfService _zplToPdfService;
        private IExpeditionOrderRepository _expeditionOrderRepository;
        private readonly IExpeditionOrderItemsRepository _expeditionOrderItemsRepository;
        private readonly IExpeditionOrderService _expeditionOrderService;
        private readonly IExpeditionOrderTagShippingRepository _expeditionOrderTagShippingRepository;

        public PickingListService(IPickingListRepository pickingListRepository,
            IPackingRepository packingRepository,
            ILabelaryAPIService labelaryAPIService,
            IReceiptNoteRepository receiptNoteRepository,
            ISimplifiedDanfeService simplifiedDanfeService,
            IInvoiceService invoiceService,
            IZplToPdfService zplToPdfService,
            IPickingListItemRepository pickingListItemRepository,
            IExpeditionOrderItemsRepository expeditionOrderItemsRepository,
            IExpeditionOrderService expeditionOrderService,
            IExpeditionOrderTagShippingRepository expeditionOrderTagShippingRepository,
            IExpeditionOrderRepository expeditionOrderRepository)
        {
            _pickingListRepository = pickingListRepository;
            _packingRepository = packingRepository;
            _labelaryAPIService = labelaryAPIService;
            _receiptNoteRepository = receiptNoteRepository;
            _simplifiedDanfeService = simplifiedDanfeService;
            _invoiceService = invoiceService;
            _zplToPdfService = zplToPdfService;
            _pickingListItemRepository = pickingListItemRepository;
            _expeditionOrderItemsRepository = expeditionOrderItemsRepository;
            _expeditionOrderService = expeditionOrderService;
            _expeditionOrderTagShippingRepository = expeditionOrderTagShippingRepository;
            _expeditionOrderRepository = expeditionOrderRepository;
        }

        public bool CancelById(int id)
        {


            bool returned = _pickingListRepository.CancelById(id);

            if (returned == false)
                throw new Exception("Não foi possível excluir o item");

            return true;
        }

        public async Task<byte[]> GenerateSimplifiedDanfePickingList(List<int> pickingOrders)
        {
            List<FileConvert> generaterdFiles = new List<FileConvert>();


            foreach (var pickingOrder in pickingOrders)
            {
                PickingList picking = await _pickingListRepository.GetByIdAsync(pickingOrder);

                if (picking == null)
                {
                    throw new Exception($"Não foi possíve encontrar a separação de numero {pickingOrder}");
                }

                var generaterdFile = await _expeditionOrderService.GenerateFilesConvert(picking.ExpeditionOrder);


                generaterdFiles.AddRange(generaterdFile);
            }
            
            var data = await _zplToPdfService.ConvertFilesToPDFs(generaterdFiles);

            var pdfMarged = _zplToPdfService.MargeSimplifiedDanfesPdfsFiles(data);

            byte[] byteData = pdfMarged.GetByteByPdfDocument();

            return byteData;
        }       
              

        public List<PickingList> GetWithStatusGeradoAndEmAndamento()
        {
            return _pickingListRepository.GetAllWithStatusGeradoAndEmAtendimento();
        }

        public PagedList<PickingListWithUrlAndArrayByteViewModel> GetAllPaged(int Page, int? CartId, int? InvoiceNumber)
        {
            int PageSize = 100;

            return _pickingListRepository.GetAllPaged(Page,PageSize, CartId, InvoiceNumber);
        }

        public PickingList GetLastByCartId(int cartId)
        {
            return _pickingListRepository.GetLastByCartId(cartId);
        }

        public async Task Cancel(int id)
        {
            PickingList picking = await _pickingListRepository.GetByIdAsync(id);
            if (picking == null)
                throw new Exception("Ocorreu um problema ao tentar cancelar essa lista de separação, por favor, tente novamente!");

            if(picking.Status == PickingListStatus.Finalizado)
                throw new Exception("Não é possível cancelar uma lista de separação que já foi finalizada");


            foreach (var order in picking.ExpeditionOrder)
            {
                order.PickingListId = null;
                order.Status = ExpeditionOrderStatus.Processed;
                
;               await _expeditionOrderRepository.UpdateAsync(order);
            }

            picking.Status = PickingListStatus.Cancelado;
            _pickingListRepository.Update(picking);

            await _pickingListItemRepository.CancelByPickingListIdAsync(picking.Id);

        }

        public async Task<List<List<string>>> CheckIfExistsMelhorEnvioOrder(List<int> ids)
        {
            Dictionary<int, List<string>> clientIdToOrders = new Dictionary<int, List<string>>();

            foreach (var pickingOrder in ids)
            {
                PickingList picking = await _pickingListRepository.GetByIdAsync(pickingOrder);

                if (picking == null)
                {
                    throw new Exception($"Não foi possível encontrar a separação de número {pickingOrder}");
                }

                foreach (var order in picking.ExpeditionOrder)
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
            }

            return clientIdToOrders.Values.ToList();
        }

        public async Task<List<PickingListExpeditionOrdersViewModel>> GetOrdersByPickingListId(int id)
        {
            var pickingList = await _pickingListRepository.GetByIdAsync(id);

            var ordersConverted = ConvertExpeditionListToPickingListExpeditionOrdersViewModel(pickingList.ExpeditionOrder);

            return ordersConverted;
        }

        private List<PickingListExpeditionOrdersViewModel> ConvertExpeditionListToPickingListExpeditionOrdersViewModel(List<ExpeditionOrder> orders)
        {
            var response = new List<PickingListExpeditionOrdersViewModel>();

            foreach (var order in orders)
            {
                var method = (ShippingMethodEnum) order.ShippingMethod;
                response.Add(new PickingListExpeditionOrdersViewModel()
                {
                    Id = order.Id,
                    InvoiceAccessKey = order.InvoiceAccessKey,
                    InvoiceNumber = order.InvoiceNumber,
                    SocialReason = order.Client?.SocialReason,
                    MarketPlace = method.GetDescription()
                });
            }

            return response;
        }
    }
}
