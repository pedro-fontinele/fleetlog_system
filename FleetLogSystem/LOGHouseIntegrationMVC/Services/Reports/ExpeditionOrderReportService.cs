using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Migrations;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.Util;
using LOGHouseSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using MimeKit.Cryptography;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.Services.Reports
{
    public class ExpeditionOrderReportService : IExpeditionOrderReportService
    {
        private IExpeditionOrderRepository _expeditionOrderRepository;
        private IClientContractsRepository _clientContractsRepository;
        private IPackingRepository _packingRepository;
        private IExpeditionOrderHistoryRepository _expeditionOrderHistoryRepository;
        private IPackingHistoryRepository _packingHistoryRepository;
        private IPickingListHistoryRepository _pickingListHistoryRepository;
        private IClientsRepository _clientsRepository;

        public ExpeditionOrderReportService(
            IExpeditionOrderRepository expeditionOrderRepository,
            IClientContractsRepository clientContractsRepository,
            IPackingRepository packingRepository,
            IExpeditionOrderHistoryRepository expeditionOrderHistoryRepository,
            IPickingListHistoryRepository pickingListHistoryRepository,
            IClientsRepository clientsRepository,
            IPackingHistoryRepository packingHistoryRepository)
        {
            _expeditionOrderRepository = expeditionOrderRepository;
            _clientContractsRepository = clientContractsRepository;
            _packingRepository = packingRepository;
            _expeditionOrderHistoryRepository = expeditionOrderHistoryRepository;
            _pickingListHistoryRepository = pickingListHistoryRepository;
            _clientsRepository = clientsRepository;
            _packingHistoryRepository = packingHistoryRepository;
        }
        public async Task<byte[]> GenerateReport(ExpeditionOrderReportViewModel model)
        {

            ValidateModel(model);

            if (model.UserLoged.PermissionLevel == Infra.Enums.PermissionLevel.Client)
            {
                model.ClientId = _clientsRepository.GetByUserId(model.UserLoged.Id).Id;
            }

            List<ExpeditionOrderStatus> statuses = new List<ExpeditionOrderStatus>
            {
                ExpeditionOrderStatus.Separated,
                ExpeditionOrderStatus.Processed,
                ExpeditionOrderStatus.InPacking,
                ExpeditionOrderStatus.Packed,
                ExpeditionOrderStatus.InPickingList,
                ExpeditionOrderStatus.Dispatched
            };


            var query = _expeditionOrderRepository.GetQueryableFilter(new ExpeditionOrderFilterViewModel()
            {
                ClientId = model.ClientId,
                FinalizeStartDate = model.FinalizeStartDate,
                FinalizeEndDate = model.FinalizeEndDate,
                CreationStartDate = model.CreationStartDate,
                CreationEndDate = model.CreationEndDate,
                Status = model.Status
            }, statuses);

            var orders = await query.Include(e => e.ExpeditionOrderItems)                
                .Include(e => e.Client)                
                .ToListAsync();

            var orderQuantity = orders.Count;

            if (orderQuantity == 0)
            {
                throw new Exception("Nenhuma expedição encontrada.");
            }

            //required using ClosedXML.Excel;
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("Expedição");

                AddReportHeader(model, worksheet);

                for (int index = 1; index <= orderQuantity; index++)
                {
                    await AddDataLine(index, model, worksheet, orders[index - 1]);

                    AddReportAlign(index, model, worksheet);
                }
                //required using System.IO;  
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
                }
            }
        }

        public void ValidateModel(ExpeditionOrderReportViewModel model)
        {
            if (model.FinalizeStartDate != null && model.FinalizeEndDate != null && (model.FinalizeStartDate > model.FinalizeEndDate))
            {
                throw new Exception("A data inicial de finalização precisa ser menor que a data final de finalização.");
            }

            if (model.CreationStartDate != null && model.CreationEndDate != null && (model.CreationStartDate > model.CreationEndDate))
            {
                throw new Exception("A data inicial de movimentação precisa ser menor que a data final de movimentação");
            }
        }
    
        public void AddReportHeader(ExpeditionOrderReportViewModel model, IXLWorksheet worksheet)
        {
            worksheet.Cell(1, 1).Value = "Id Expedição";
            worksheet.Cell(1, 2).Value = "Cliente";
            worksheet.Cell(1, 3).Value = "CNPJ";
            worksheet.Cell(1, 4).Value = "Chave da NF";
            worksheet.Cell(1, 5).Value = "Numero da Nota Fiscal";
            worksheet.Cell(1, 6).Value = "Quantidade Expedida";
            worksheet.Cell(1, 7).Value = "Valor total da Nota Fiscal";
            worksheet.Cell(1, 8).Value = "Quantidade de Picking / Packing";
            worksheet.Cell(1, 9).Value = "Status";
            worksheet.Cell(1, 10).Value = "Data de Entrada";
            worksheet.Cell(1, 11).Value = "Data Saida";
            if (model.UserLoged.PermissionLevel != PermissionLevel.Client)
            {
                worksheet.Cell(1, 12).Value = "Usuario Integração";
                worksheet.Cell(1, 13).Value = "Usuario Separação";
                worksheet.Cell(1, 14).Value = "Usuario Bipagem Separação";
                worksheet.Cell(1, 15).Value = "Usuario Empacotamento";
                worksheet.Cell(1, 16).Value = "Usuario Expedição";
            }
        }

        public void AddReportAlign(int index, ExpeditionOrderReportViewModel model, IXLWorksheet worksheet)
        {
            worksheet.Cell(index + 1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(index + 1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(index + 1, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(index + 1, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(index + 1, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(index + 1, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(index + 1, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(index + 1, 8).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(index + 1, 9).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(index + 1, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(index + 1, 11).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            if (model.UserLoged.PermissionLevel != PermissionLevel.Client)
            {
                worksheet.Cell(index + 1, 12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(index + 1, 13).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(index + 1, 14).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(index + 1, 15).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(index + 1, 16).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
        }

        public async Task AddDataLine(int index, ExpeditionOrderReportViewModel model, IXLWorksheet worksheet, ExpeditionOrder order)
        {
            var contract = await _clientContractsRepository.GetByClientId(order.ClientId);

            var itensQuantity = order.ExpeditionOrderItems.Sum(e => e.Quantity);

            worksheet.Cell(index + 1, 1).Value = order.Id;

            if (order.FinalizeDate != null)
            {
                worksheet.Cell(index + 1, 2).Value = order.FinalizeDate.Value.Date.ToString("dd/MM/yyyy hh:ii:ss");
            }
            else
            {
                worksheet.Cell(index + 1, 2).Value = "";
            }

            
            worksheet.Cell(index + 1, 2).Value = order.Client.SocialReason;
            worksheet.Cell(index + 1, 3).Value = order.Client.Cnpj.ToCNPJ();
            worksheet.Cell(index + 1, 4).Value = order.InvoiceAccessKey.ToAccessKey();
            worksheet.Cell(index + 1, 5).Value = order.InvoiceNumber;
            worksheet.Cell(index + 1, 6).Value = itensQuantity;

            var sumValues = order.ExpeditionOrderItems.Select(e => e.Value * e.Quantity).Sum();

            worksheet.Cell(index + 1, 7).Value = sumValues;

            var shippingUnits = 1;

            if (contract.ShippingUnits != null && contract.ShippingUnits > 0)
                shippingUnits = Convert.ToInt32(contract.ShippingUnits);

            worksheet.Cell(index + 1, 8).Value = Math.Ceiling(itensQuantity / shippingUnits);

            var attribute = order.Status.GetAttributeOfType<DisplayAttribute>();

            worksheet.Cell(index + 1, 9).Value = attribute != null ? attribute.Name : "Não encontrado";

            worksheet.Cell(index + 1, 10).Value = order.IssueDate;//order.;

            worksheet.Cell(index + 1, 11).Value = order.FinalizeDate;//order.;

            if (model.UserLoged.PermissionLevel != PermissionLevel.Client)
            {

                List<ExpeditionOrderHistory> histories = await _expeditionOrderHistoryRepository.GetByOrderIdAsync(order.Id);
                
                var integrationUser =  histories?.OrderByDescending(e => e.Date)?.FirstOrDefault(x => x.Status == ExpeditionOrderStatus.Processed);
                worksheet.Cell(index + 1, 12).Value = integrationUser?.User != null ? integrationUser.User.Name : "Não encontrado";

                if (order.PickingListId != null)
                {

                    
                    var pickingUser = histories?.OrderByDescending(e => e.Date)?.FirstOrDefault(x => x.Status == ExpeditionOrderStatus.InPickingList);
                    worksheet.Cell(index + 1, 13).Value = pickingUser?.User != null ? pickingUser.User.Name : "Não encontrado";

                    
                    var pickingBeppingUser = histories?.OrderByDescending(e => e.Date)?.FirstOrDefault(x => x.Status == ExpeditionOrderStatus.BeepingPickingList);
                    worksheet.Cell(index + 1, 14).Value = pickingBeppingUser?.User != null ? pickingBeppingUser.User.Name : "Não encontrado";
                }
                else
                {
                    worksheet.Cell(index + 1, 13).Value = "Não encontrado";
                    worksheet.Cell(index + 1, 14).Value = "Não encontrado";
                }


                Packing packing = _packingRepository.GetByExpeditionOrderId(order.Id);

                if(packing != null)
                {
                    List<PackingHistory> packings = await _packingHistoryRepository.GetAllByPackingId(packing.Id);

                    var InPacking = packings?.OrderByDescending(e => e.Date)?.FirstOrDefault(x => x.Status == PackingStatus.Gerado);
                    worksheet.Cell(index + 1, 15).Value = InPacking?.User != null ? InPacking.User.Name : "Não encontrado";
                }
                else
                {
                    worksheet.Cell(index + 1, 15).Value = "Não encontrado";
                }

                var packingDispached = histories?.OrderByDescending(e => e.Date)?.FirstOrDefault(x => x.Status == ExpeditionOrderStatus.Dispatched);
                worksheet.Cell(index + 1, 16).Value = packingDispached?.User != null ? packingDispached.User.Name : "Não encontrado";
            }
        }
    }
}
