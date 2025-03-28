using LOGHouseSystem.Controllers.API.BarcodeColectorApi;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PagedList;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IReceiptNoteLotsRepository
    {
        public List<ReceiptNoteLots> GetByProductId(int id);

        public ReceiptNoteLots GetLotById(int id);

        Task<ReceiptNoteLots> GetLotByIdAsync(int id);

        public ReceiptNoteLots Add(ReceiptNoteLots lot);

        public IQueryable<ReceiptNoteLots> GetAll();

        public ReceiptNoteLots Update(ReceiptNoteLots lot);
        Task<ReceiptNoteLots> UpdateAsync(ReceiptNoteLots lot);

        public List<ReceiptNoteLots> GetByProductIdAndStatus(int productId, LotStatus status);
        Task<List<ReceiptNoteLots>> GetByProductIdAndStatusAsync(int productId, LotStatus status);

        Task<PaginationBase<ReceiptNoteLots>> GetAllAsync(ReceiptNoteIndexLotsPaginationRequest request);

    }
}
