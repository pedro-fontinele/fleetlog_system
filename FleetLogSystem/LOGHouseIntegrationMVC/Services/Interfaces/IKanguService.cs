using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IKanguService
    {
        Task CreateIntegrationVariables(int integrationId);
        Task DownloadShippment(ExpeditionOrder order);
    }
}
