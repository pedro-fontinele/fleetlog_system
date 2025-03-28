using ClosedXML.Excel;
using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IReceiptNoteReportService
    {
        Task<byte[]> GenerateReport(ReceiptNoteReportViewModel model);
    }
}
