using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services
{
    public interface IMelhorEnvioService
    {
        Task GetAllReleasedShippingData(int clientId);        
        Task DownloadShippment(ExpeditionOrder order);
        List<IntegrationVariable> CreateIntegrationVariables(int integrationId, int clientId);
    }
}
