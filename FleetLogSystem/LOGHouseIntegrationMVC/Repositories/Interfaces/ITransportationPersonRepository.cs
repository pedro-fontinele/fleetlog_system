using LOGHouseSystem.Models;
using LOGHouseSystem.Infra.Pagination;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface ITransportationPersonRepository
    {
        Task<TransportationPerson> GetById(int id);
        Task<TransportationPerson> AddAsync(TransportationPerson transportationPerson);
        Task<TransportationPerson> UpdateAsync(TransportationPerson transportationPerson);
        Task<PaginationBase<TransportationPerson>> GetByPagination(PaginationRequest request);
        TransportationPerson GetByCpf(string cpf);
    }
}
