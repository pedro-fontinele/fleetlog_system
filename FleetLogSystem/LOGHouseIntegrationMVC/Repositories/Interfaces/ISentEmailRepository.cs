using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using PagedList;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface ISentEmailRepository
    {
        Task<List<SentEmail>> GetAllAsync();

        Task<List<SentEmail>> GetAllByClientIdAsync(int id);

        Task<SentEmail> GetByIdAsync(int id);

        SentEmail Add(SentEmail email);

        PagedList<SentEmailViewModel> GetByFilter(FilterSentEmailViewModel filter);
    }
}
