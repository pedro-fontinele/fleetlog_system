using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IIntegrationRepository
    {
        List<Integration> GetByClientLoged();
        Integration Add(Integration integration);
        Integration GetById(int id);
        Task<Integration> GetByClientIdAndNameAsync(int clientId, string name);
        Task<List<Integration>> GetAllIntegrationsByName(List<string> list);
        Task DeleteById(int id);
    }
}
