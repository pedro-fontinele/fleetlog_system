using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IIntegrationService
    {
        Integration CreateNewIntegration(Integration integration);
        Task<bool> CheckIfIntegrationAlreadyExist(Integration integration);
        Task DeleteIntegration(int id);
    }
}
