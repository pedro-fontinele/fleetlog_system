using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class IntegrationVariableRepository : RepositoryBase, IIntegrationVariableRepository
    {

        public IntegrationVariable Add(IntegrationVariable integrationVariable)
        {
            _db.IntegrationVariables.Add(integrationVariable);
            _db.SaveChanges();

            return integrationVariable;
        }        

        public void AddRange(List<IntegrationVariable> integrationVariables)
        {
            _db.IntegrationVariables.AddRange(integrationVariables);
            _db.SaveChanges();
        }

        public IntegrationVariable GetById(int id)
        {
            return _db.IntegrationVariables
                      .FirstOrDefault(x => x.Id == id);
        }

        public List<IntegrationVariable> GetByIntegrationId(int integrationId)
        {
            return _db.IntegrationVariables.Where(iv => iv.IntegrationId == integrationId).ToList();
        }

        public IntegrationVariable GetByIntegrationIdAndName(int integrationId, string name)
        {
            return _db.IntegrationVariables.FirstOrDefault(iv => iv.IntegrationId == integrationId && iv.Name == name);
        }

        public async Task<IntegrationVariable> GetByIntegrationIdAndNameAsync(int integrationId, string name)
        {
            return await _db.IntegrationVariables.Where(iv => iv.IntegrationId == integrationId && iv.Name == name).FirstOrDefaultAsync();
        }

        public async Task<List<IntegrationVariable>> GetByIntegrationIdAsync(int integrationId)
        {
            return await _db.IntegrationVariables.Where(iv => iv.IntegrationId == integrationId).ToListAsync();
        }

        public IntegrationVariable Update(IntegrationVariable integrationVariable)
        {
            _db.Entry(integrationVariable).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _db.SaveChanges();

            return integrationVariable;
        }

        public IntegrationVariable UpdateValue(int id, string value)
        {
            IntegrationVariable variableById = GetById(id);
            if (variableById == null)
                throw new ArgumentException("Não foi possível atualizar a integração.");

            variableById.Value = value;

            _db.Update(variableById);
            _db.SaveChanges();

            return variableById;
        }

        public async Task<IntegrationVariable> AddAsync(IntegrationVariable integrationVariable)
        {
            _db.IntegrationVariables.Add(integrationVariable);
            await _db.SaveChangesAsync();

            return integrationVariable;
        }

        public async Task<IntegrationVariable> UpdateAsync(IntegrationVariable integrationVariable)
        {
            _db.Entry(integrationVariable).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _db.SaveChanges();

            return integrationVariable;
        }

        public async Task AddRangeAsync(List<IntegrationVariable> integrationVariables)
        {
            _db.IntegrationVariables.AddRange(integrationVariables);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAllByIntegrationId(int id)
        {
            var items = await _db.IntegrationVariables.Where(e => e.IntegrationId == id).ToListAsync();
            _db.IntegrationVariables.RemoveRange(items);
            await _db.SaveChangesAsync();
        }
    }
}
