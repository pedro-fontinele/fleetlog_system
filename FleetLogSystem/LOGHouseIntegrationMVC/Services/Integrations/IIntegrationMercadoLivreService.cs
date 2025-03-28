using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services.Integrations
{
    public interface IIntegrationMercadoLivreService
    {
        Task DownlodShippment(ExpeditionOrder order, bool secondTrying = false);
        Task ConfigMLAccessToken(int integrationId, string code);
        List<IntegrationVariable> CreateIntegrationVariables(int integrationId);
        Task<bool> SaveRefreshToken(int clientId);
    }
}
