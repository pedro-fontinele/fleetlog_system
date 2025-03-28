using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class ClientContractsRepository : RepositoryBase, IClientContractsRepository
    {

        public ClientContract Add(ClientContract clientContract)
        {

            if (clientContract.ExcessOrderValue == null)
                clientContract.ExcessOrderValue = 5.50m;

            if (clientContract.InsurancePercentage == null)
                clientContract.InsurancePercentage = 0.50m;

            _db.ClientContracts.Add(clientContract);
            _db.SaveChanges();

            return clientContract;
        }

        public async Task<ClientContract> GetByClientId(int? clientId)
        {
            var clientContract = await _db.ClientContracts.Where(e => e.ClientId == clientId).FirstOrDefaultAsync();

            return clientContract;
        }
    }
}
