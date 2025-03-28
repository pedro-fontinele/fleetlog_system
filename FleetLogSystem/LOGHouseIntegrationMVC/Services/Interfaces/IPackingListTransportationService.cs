using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request;
using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Response;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using PagedList;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IPackingListTransportationService
    {
        Task<PackingListTransportationResponse> Add(PackingListTransportationRequest request,int? userId);

        PagedList<PackingListTransportation> GetAllPaged(int Page);
        Task SignTransportation(int id, List<IFormFile> signImage, int userId);
        Task SaveTransportPlate(int id, List<IFormFile> plateImage);

        PagedList<PackingListTransportation> GetByFilter(FilterPackingListTransportationViewModel filter);
    }
}
