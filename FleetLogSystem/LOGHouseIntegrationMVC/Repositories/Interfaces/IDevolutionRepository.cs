using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using PagedList;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IDevolutionRepository
    {
        Task<Devolution> GetById(int id);

        Devolution GetDevolutionById(int id);

        PagedList<DevolutionCreateAndUpdateViewModel> GetAllPaged(int page, int pageSize);

        PagedList<DevolutionCreateAndUpdateViewModel> GetAllPagedByUserLoged(int page, int pageSize, int clientId);

        Task<Devolution> AddAsync(Devolution devolution);

        Task<Devolution> UpdateAsync(Devolution devolution);

        Devolution Update(Devolution devolution);

        Task<bool> Delete(int id);

        IQueryable<DevolutionCreateAndUpdateViewModel> GetQueryableFilter(FilterDevolutionViewModel filter);

        PagedList<DevolutionCreateAndUpdateViewModel> GetByFilters(FilterDevolutionViewModel filter);
    }
}
