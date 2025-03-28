using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using PagedList;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IPackingService
    {
        List<Packing> GetWithStatusEmAtendimentoAndGerado();

        PagedList<Packing> GetAllPaged(int Page);

        Packing AddByOrderExpedition(int id, int userId);

        Task<bool> DeleteById(int id);

        Task SendPackingImage(int id, List<IFormFile> formFile, string imageBase64 = "");

        Packing SearchByAccessKey(string invoiceAccessKey);

        Task<Packing> SearchByAccessKeyAsync(string invoiceAccessKey);
        List<Packing> GeneratePackingByPickingId(int id, int userId);

        List<Packing> AddByPickingLists(string[] selecteds);


        PagedList<Packing> GetPackingWhithoutPackingListTrasportationByFilter(FilterPackingWhithoutPackingListTrasportationViewModel filter);


        PagedList<Packing> GetByFilter(FilterPackingViewModel filter);

        
    }
}
