using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using PagedList;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IPackingListTransportationRepository
    {
        Task<PackingListTransportation> AddAsync(PackingListTransportation request);
        List<PackingListTransportation> GetByStatus(PackingListTransportationStatus status);
        PagedList<PackingListTransportation> GetAllPaged(int page, int pageSize);
        PackingListTransportation? GetById(int id);
        Task<PackingListTransportation> UpdateAsync(PackingListTransportation packingListTransportation);
        object GetIfContainsPrefix(string prefix);
        PagedList<PackingListTransportation> GetByFilters(FilterPackingListTransportationViewModel filter);
    }
}
