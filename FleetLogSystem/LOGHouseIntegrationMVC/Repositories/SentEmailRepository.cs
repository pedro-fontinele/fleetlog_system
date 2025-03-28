using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using PagedList;

namespace LOGHouseSystem.Repositories
{
    public class SentEmailRepository : RepositoryBase, ISentEmailRepository
    {
        public SentEmail Add(SentEmail email)
        {
            if (email == null) throw new ArgumentNullException("Não foi possível salvar o Email na base pois ele está nulo");

           _db.SentEmails.Add(email);
           _db.SaveChanges();

            return email;
        }

        public async Task<List<SentEmail>> GetAllAsync()
        {
            return await _db.SentEmails
                            .AsNoTracking()
                            .Include(x => x.Client)
                            .OrderByDescending(x => x.SendData)
                            .ToListAsync();

        }

        public async Task<List<SentEmail>> GetAllByClientIdAsync(int id)
        {
            return await _db.SentEmails
                            .AsNoTracking()
                            .Where(x => x.ClientId == id)
                            .Include(x => x.Client)
                            .ToListAsync();
                            
        }

        public PagedList<SentEmailViewModel> GetByFilter(FilterSentEmailViewModel filter)
        {
            int PageSize = 50;

            var query = GetQueryableFilter(filter);

            return (PagedList<SentEmailViewModel>)query
                .AsNoTracking()
                //.Include(x => x.ReceiptNoteItems)
                .ToPagedList(filter.Page, PageSize);
        }

        public IQueryable<SentEmailViewModel> GetQueryableFilter(FilterSentEmailViewModel filter)
        {
            var query = _db.SentEmails
                .Include(x => x.Client)
                .Select(sent => new SentEmailViewModel
                {
                    Id = sent.Id,
                    Client = sent.Client,
                    Title = sent.Title,
                    Body = sent.Body,
                    To = sent.To,
                    ToEmail = sent.ToEmail,
                    SendData = sent.SendData,
                    ClientId = sent.ClientId,
                    InvoiceNumber = sent.InvoiceNumber
                })
                .AsQueryable();

            if (filter.ClientId != null)
                query = query.Where(x => x.ClientId == filter.ClientId);


            if (filter.SendData.HasValue)
            {
                DateTime date = filter.SendData.Value.Date;
                query = query.Where(x => EF.Functions.DateDiffDay(x.SendData, date) == 0);
            }

            if (filter.Title != null)
                query = query.Where(x => x.Title == filter.Title);

            if (filter.ToEmail != null)
                query = query.Where(x => x.ToEmail == filter.ToEmail);

            query = query.OrderByDescending(x => x.Id);

            return query;
        }

        public async Task<SentEmail> GetByIdAsync(int id)
        {
            if(id <= 0) throw new ArgumentOutOfRangeException("Não foi possível encontrar a informação desejada, por favor, tente novamente informado um item válido");

            return await _db.SentEmails
                       .Include(x => x.Client)
                      .FirstOrDefaultAsync(x => x.Id == id);
                      
        }
    }
}
