using LOGHouseSystem.Controllers.ViewComponents;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using PagedList;
using System.Net.Mail;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IDevolutionService
    {
        Task<Devolution> GetByIdAsync(int id);

        Task<DevolutionCreateAndUpdateViewModel> GetViewmodelByIdAsync(int id, List<DevolutionAndProduct> list);

        PagedList<DevolutionCreateAndUpdateViewModel> GetAllPaged(int Page);

        PagedList<DevolutionCreateAndUpdateViewModel> GetAllPagedByUserLoged(int Page);

        Task<Devolution> AddAsync(DevolutionCreateAndUpdateViewModel note);

        Task<Devolution> UpdateAsync(DevolutionCreateAndUpdateViewModel note);

        Task<bool> DeleteAsync(int id);

        Task SendDevolutionImage(int id, List<IFormFile> formFile, string imageBase64 = "");

        Task<List<DevolutionImage>> GetAllImagesByIdAsync(int id);

        List<int> GetProductsIdList(string json);

        void SendDevolutionEmail(Devolution dev, List<DevolutionAndProduct> products, List<byte[]> images);

        PagedList<DevolutionCreateAndUpdateViewModel> GetByFilter(FilterDevolutionViewModel filter);
    }
}
