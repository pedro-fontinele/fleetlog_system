using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IClientContractsRepository
    {
        ClientContract Add(ClientContract clientContract);
        Task<ClientContract> GetByClientId(int? clientId);
    }
}
