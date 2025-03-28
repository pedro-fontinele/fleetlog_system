using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PagedList;
using System.Data.SqlClient;

namespace LOGHouseSystem.Repositories
{
    public class RepositoryBase
    {

        protected readonly AppDbContext _db;
        protected SessionHelper _session = new SessionHelper();

        public RepositoryBase(AppDbContext db = null)
        {
            _db = db ?? new AppDbContext();
        }

        public async Task<PaginationBase<T>> PaginateQuery<T>(IQueryable<T> query, ExpeditionOrderFilterViewModel filter)
        {
            PaginationBase<T> pagination = new PaginationBase<T>();

            // var pageNumber = filter.PageNumber == 0 ? 1 : filter.PageNumber;
            // var pageSize = filter.PageSize == 0 ? 10 : filter.PageSize;

            filter.PageSize = 100;
            pagination.PageNumber = filter.PageNumber;

            var pagedData = query.ToPagedList(pagination.PageNumber, filter.PageSize);
            pagination.FirstRegisterInActualPage = pagedData.FirstItemOnPage;
            pagination.LastRegisterInActualPage = pagedData.LastItemOnPage;
            pagination.TotalRegisters = pagedData.TotalItemCount;
            pagination.Data = pagedData.ToList();

            //if (filter.PageNumber != 1)
            //{
            //    pagination.Data = await query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();
            //}
            //else
            //{
            //    pagination.Data = await query.Take(filter.PageSize).ToListAsync();
            //}

            //if (pagination.Data.Count() < 10)
            //{
            //    pagination.Data = await query.ToListAsync();

            //}

            //pagination.TotalPages = (await query.ToListAsync()).Count() / filter.PageSize;
            pagination.TotalPages = pagedData.PageCount;


            return pagination;
        }

        public async Task<PaginationBase<T>> PaginateQueryWithRequest<T>(IQueryable<T> query, PaginationRequest request)
        {
            PaginationBase<T> pagination = new PaginationBase<T>();

            request.PageSize = 10;
            
            pagination.PageNumber = request.PageNumber;

            if (pagination.PageNumber <= 0)
            {
                pagination.PageNumber = 1;
            }

            var pagedData = query.ToPagedList(pagination.PageNumber, request.PageSize);

            pagination.Data = pagedData.ToList();

            pagination.PageSize = request.PageSize;
            pagination.TotalPages = pagedData.PageCount;


            return pagination;
        }

    }
}
