using DocumentFormat.OpenXml.Spreadsheet;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class IntegrationRepository : RepositoryBase, IIntegrationRepository
    {
        private SessionHelper _session = new SessionHelper();

        public Integration Add(Integration integration)
        {
            User userLoged = _session.SearchUserSession();

            if (userLoged == null || userLoged.Id <= 0)
                throw new Exception("Erro ao buscar usuário");


            Client client = _db.Clients.FirstOrDefault(x => x.UserId == userLoged.Id);

            integration.ClientId = client.Id;
            integration.Client = client;

            _db.Add(integration);
            _db.SaveChanges();

            return integration;
        }

        public List<Integration> GetByClientLoged()
        {
            User userLoged = _session.SearchUserSession();

            if (userLoged == null || userLoged.Id <= 0)
                throw new Exception("Erro ao buscar usuário");


            Client client = _db.Clients.FirstOrDefault(x => x.UserId == userLoged.Id);

            if (client == null)
                throw new Exception("Erro ao buscar cliente");

            return _db.Integrations
                .Where(x => x.ClientId == client.Id)
                .AsNoTracking()
                .ToList();
        }

        public Integration GetById(int id)
        {
            return _db.Integrations
                       .FirstOrDefault(x => x.Id == id);
                       
        }

        public async Task<Integration> GetByClientIdAndNameAsync(int clientId, string name)
        {
            return await _db.Integrations.Where(e => e.ClientId == clientId && e.Name == name).FirstOrDefaultAsync();
        }

        public async Task<List<Integration>> GetAllIntegrationsByName(List<string> list)
        {
            return await _db.Integrations.Where(e => list.Any(a => a == e.Name)).ToListAsync();
        }

        public async Task DeleteById(int id)
        {
            var items = await _db.Integrations.Where(e => e.Id == id).FirstOrDefaultAsync();
            _db.Integrations.Remove(items);
            await _db.SaveChangesAsync();
        }
    }
}
