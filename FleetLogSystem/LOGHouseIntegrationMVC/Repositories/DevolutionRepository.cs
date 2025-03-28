using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Tsp;
using PagedList;

namespace LOGHouseSystem.Repositories
{
    public class DevolutionRepository : RepositoryBase, IDevolutionRepository
    {
        public async Task<Devolution> AddAsync(Devolution note)
        {
            await _db.Devolutions.AddAsync(note);
            await _db.SaveChangesAsync();
            return note;
        }

        public async Task<bool> Delete(int id)
        {
            Devolution dev = await GetById(id);

            if(dev != null)
            {
                _db.Devolutions.Remove(dev);
                await _db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public PagedList<DevolutionCreateAndUpdateViewModel> GetAllPaged(int page, int pageSize)
        {
            return (PagedList<DevolutionCreateAndUpdateViewModel>)_db.Devolutions
                          .AsNoTracking()
                          .Include(x => x.Client)
                          .OrderByDescending(x => x.Id)
                          .Select(devolution => new DevolutionCreateAndUpdateViewModel
                          {
                              Id = devolution.Id,
                              SenderName = devolution.SenderName,
                              InvoiceNumber = devolution.InvoiceNumber,
                              PostNumber = devolution.PostNumber,
                              Observation = devolution.Observation,
                              Status = devolution.Status,
                              EntryDate = devolution.EntryDate,
                              ClientId = devolution.ClientId,
                              ClientName = devolution.Client.SocialReason
                          })
                          .ToPagedList(page, pageSize);
        }

        public PagedList<DevolutionCreateAndUpdateViewModel> GetAllPagedByUserLoged(int page, int pageSize, int clientId)
        {
            return (PagedList<DevolutionCreateAndUpdateViewModel>)_db.Devolutions
                         .AsNoTracking()
                         .Where(X => X.ClientId == clientId)
                         .Include(x => x.Client)
                         .OrderByDescending(x => x.Id)
                         .Select(devolution => new DevolutionCreateAndUpdateViewModel
                         {
                             Id = devolution.Id,
                             SenderName = devolution.SenderName,
                             InvoiceNumber = devolution.InvoiceNumber,
                             PostNumber = devolution.PostNumber,
                             Observation = devolution.Observation,
                             Status = devolution.Status,
                             EntryDate = devolution.EntryDate,
                             ClientId = devolution.ClientId,
                             ClientName = devolution.Client.SocialReason,
                             
                         })
                         .ToPagedList(page, pageSize);
        }

        public async Task<Devolution> GetById(int id)
        {
           return await _db.Devolutions.Include(x => x.Client).Include(x => x.Products).Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id);
        }

        public Devolution GetDevolutionById(int id)
        {
            return _db.Devolutions.Include(x => x.Client).Include(x => x.Products).Include(x => x.Images).FirstOrDefault(x => x.Id == id);
        }

        public Devolution Update(Devolution devolution)
        {
            _db.Devolutions.Update(devolution);
            _db.SaveChanges();
            return devolution;
        }

        public async Task<Devolution> UpdateAsync(Devolution devolution)
        {
            _db.Devolutions.Update(devolution);
            await _db.SaveChangesAsync();
            return devolution;
        }

        public PagedList<DevolutionCreateAndUpdateViewModel> GetByFilters(FilterDevolutionViewModel filter)
        {
            int PageSize = 50;

            var query = GetQueryableFilter(filter);

            return (PagedList<DevolutionCreateAndUpdateViewModel>)query
                .AsNoTracking()
                //.Include(x => x.ReceiptNoteItems)
                .ToPagedList(filter.Page, PageSize);
        }

        public IQueryable<DevolutionCreateAndUpdateViewModel> GetQueryableFilter(FilterDevolutionViewModel filter)
        {
            var query = _db.Devolutions
                .Include(x => x.Client)
                .Select(devolution => new DevolutionCreateAndUpdateViewModel
                {
                    Id = devolution.Id,
                    SenderName = devolution.SenderName,
                    InvoiceNumber = devolution.InvoiceNumber,
                    PostNumber = devolution.PostNumber,
                    Observation = devolution.Observation,
                    Images = devolution.Images,
                    Status = devolution.Status,
                    ClientId = devolution.ClientId,
                    ClientName = devolution.Client.SocialReason, 
                    IsCreation = false ,
                    EntryDate = devolution.EntryDate
                })
                .AsQueryable();

            if (filter.ClientId != null)
                query = query.Where(x => x.ClientId == filter.ClientId);

            
            if (filter.Date != null)
            {
                DateTime date = filter.Date.Value.Date;
                query = query.Where(x => x.EntryDate.Date == date.Date); // Aplica a comparação da data no lado do cliente
            }

            if (filter.Status != null)
                query = query.Where(x => x.Status == filter.Status);

            query = query.OrderByDescending(x => x.Id);

            return query;
        }
    }
}
