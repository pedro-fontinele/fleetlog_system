using LOGHouseSystem.Controllers.API.BarcodeColectorApi.Request;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface ITransportationPersonService
    {
        Task<TransportationPerson> Generate(CreateTransportationPersonRequest data);
        Task UpdateImage(int id, List<IFormFile> frontImage, List<IFormFile> backImage);
        Task<PaginationBase<TransportationPerson>> GetPaginationBaseAsync(PaginationRequest request);
    }
}
