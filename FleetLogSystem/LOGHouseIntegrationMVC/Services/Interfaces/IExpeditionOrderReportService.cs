using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IExpeditionOrderReportService
    {
        Task<byte[]> GenerateReport(ExpeditionOrderReportViewModel model);
    }
}
