using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IIntegrationVariableRepository
    {
        List<IntegrationVariable> GetByIntegrationId(int integrationId);
        IntegrationVariable GetByIntegrationIdAndName(int integrationId, string name);        
        IntegrationVariable Add(IntegrationVariable integrationVariable);
        IntegrationVariable Update(IntegrationVariable integrationVariable);
        void AddRange(List<IntegrationVariable> integrationVariables);
        Task AddRangeAsync(List<IntegrationVariable> integrationVariables);
        Task<List<IntegrationVariable>> GetByIntegrationIdAsync(int integrationId);
        Task<IntegrationVariable> UpdateAsync(IntegrationVariable integrationVariable);
        Task<IntegrationVariable> AddAsync(IntegrationVariable integrationVariable);
        IntegrationVariable GetById(int id);
        IntegrationVariable UpdateValue(int id, string value);
        Task<IntegrationVariable> GetByIntegrationIdAndNameAsync(int integrationId, string name);
        Task DeleteAllByIntegrationId(int id);
    }
}
