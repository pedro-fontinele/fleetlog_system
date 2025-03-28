using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using LOGHouseSystem.Infra.Pagination;

namespace LOGHouseSystem.Repositories
{
    public class TransportationPersonRepository : RepositoryBase, ITransportationPersonRepository
    {
        public async Task<TransportationPerson> GetById(int id)
        {
            return await _db.TransportationPeople.Where(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task<TransportationPerson> AddAsync(TransportationPerson transportationPerson)
        {
            _db.TransportationPeople.Add(transportationPerson);
            await _db.SaveChangesAsync();
            return transportationPerson;
        }


        public async Task<TransportationPerson> UpdateAsync(TransportationPerson transportationPerson)
        {
            _db.TransportationPeople.Update(transportationPerson);
            await _db.SaveChangesAsync();
            return transportationPerson;
        }
        
        public async Task<PaginationBase<TransportationPerson>> GetByPagination(PaginationRequest request)
        {
            var query = _db.TransportationPeople.Where(x => x.Id != 0);

            return await PaginateQueryWithRequest(query, request);
        }

        public TransportationPerson GetByCpf(string cpf)
        {
            return _db.TransportationPeople.Where(x => x.Cpf == cpf).FirstOrDefault();
        }
    }
}
