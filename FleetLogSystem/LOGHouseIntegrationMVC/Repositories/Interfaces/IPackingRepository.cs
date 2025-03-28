using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using PagedList;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IPackingRepository
    {
        List<Packing> GetWithStatusGeradoOrEmAtendimento();

        PagedList<Packing> GetAllPaged(int page, int pageSize);

        Packing GetById(int id);

        Packing Add(Packing packing);

        Task<bool> Delete(Packing packing);
        Task<List<Packing>> GetByStatusAsync(PackingStatus statusId);
        Task<Packing> GetByIdAsync(int id);
        Task<Packing> UpdateAsync(Packing packing);
        Packing GetByExpeditionOrderId(int id);
        Task<Packing> GetByExpeditionOrderIdAsync(int id, bool takeExpeditionOrder = true);

        Task<Packing> UpdateStatusAsync(Packing packing, PackingStatus status);

        PagedList<Packing> GetPackingWhithoutPackingListTrasportationByFilter(FilterPackingWhithoutPackingListTrasportationViewModel filter);
        PagedList<Packing> GetByFilters(FilterPackingViewModel filter);
       
    }
}
