using ClosedXML.Excel;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.Util;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.Services.Reports
{
    public class ReceiptNoteReportService : IReceiptNoteReportService
    {
        private IReceiptNoteRepository _receiptNoteRepository;
        private readonly IClientsRepository _clientsRepository;
        private ILabelBillingRepository _labelBillingRepository;

        public ReceiptNoteReportService(IReceiptNoteRepository receiptNoteRepository, ILabelBillingRepository labelBillingRepository, IClientsRepository clientsRepository)
        {
            _receiptNoteRepository = receiptNoteRepository;
            _labelBillingRepository = labelBillingRepository;
            _clientsRepository = clientsRepository;
        }
        public async Task<byte[]> GenerateReport(ReceiptNoteReportViewModel model)
        {

            if (model.EntryDateStart != null && model.EntryDateEnd != null && (model.EntryDateStart > model.EntryDateEnd))
            {
                throw new Exception("A data inicial precisa ser menor que a data final");
            }

            if(model.UserLoged.PermissionLevel == Infra.Enums.PermissionLevel.Client)
            {
                model.ClientId = _clientsRepository.GetByUserId(model.UserLoged.Id).Id;
            }

            var query = _receiptNoteRepository.GetQueryableFilter(new FilterViewModel()
            {
                ClientId = model.ClientId,
                EntryDateStart = model.EntryDateStart,
                EntryDateEnd = model.EntryDateEnd,
            });

            var receipts = await query.ToListAsync();

            var receiptQuantity = receipts.Count;

            if (receiptQuantity == 0)
            {
                throw new Exception("Nenhum recebimento encontrado.");
            }

            //required using ClosedXML.Excel;
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet =
                workbook.Worksheets.Add("Recebimento");
                worksheet.Cell(1, 1).Value = "Id Recebimento";
                worksheet.Cell(1, 2).Value = "Data Entrada";
                worksheet.Cell(1, 3).Value = "Cliente";
                worksheet.Cell(1, 4).Value = "CNPJ";
                worksheet.Cell(1, 5).Value = "Chave da NF";
                worksheet.Cell(1, 6).Value = "Numero da Nota Fiscal";
                worksheet.Cell(1, 7).Value = "Quantidade Recebida";
                worksheet.Cell(1, 8).Value = "Valor total da Nota Fiscal";
                worksheet.Cell(1, 9).Value = "Valor total dos produtos";                
                worksheet.Cell(1, 10).Value = "Produtos Etiquetados";
                worksheet.Cell(1, 11).Value = "Status";

                for (int index = 1; index <= receiptQuantity; index++)
                {

                    var labels = await _labelBillingRepository.GetByReceiptNoteAsync(receipts[index - 1].Id);                    

                    worksheet.Cell(index + 1, 1).Value = receipts[index - 1].Id;                    
                    worksheet.Cell(index + 1, 2).Value = receipts[index - 1].EntryDate;
                    worksheet.Cell(index + 1, 3).Value = receipts[index - 1].Client.SocialReason;
                    worksheet.Cell(index + 1, 4).Value = receipts[index - 1].Client.Cnpj.ToCNPJ();
                    worksheet.Cell(index + 1, 5).Value = receipts[index - 1].AccessKey.ToAccessKey();
                    worksheet.Cell(index + 1, 6).Value = receipts[index - 1].Number;
                    worksheet.Cell(index + 1, 7).Value = receipts[index - 1].ReceiptNoteItems.Sum(e => e.Quantity);
                    worksheet.Cell(index + 1, 8).Value = receipts[index - 1].TotalInvoiceValue.ToString().Replace('.', ',');
                    worksheet.Cell(index + 1, 9).Value = receipts[index - 1].ReceiptNoteItems.Sum(e => e.Value * Convert.ToDecimal(e.Quantity));
                    worksheet.Cell(index + 1, 10).Value = labels.Sum(e => e.Value);

                    var attribute = receipts[index - 1].Status.GetAttributeOfType<DisplayAttribute>();

                    worksheet.Cell(index + 1, 11).Value = attribute != null ? attribute.Name : "Não encontrado";                    

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
    }
}
