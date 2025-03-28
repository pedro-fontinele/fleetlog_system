using LOGHouseSystem.Models;
using LOGHouseSystem.Models.SendEmails;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using PagedList;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface ISentEmailService
    {
        bool AddEmail(EmailData emailData, User? user, int? clientId, int? expeditionOrderId = null);

        Task<List<SentEmail>> GetSentEmailsByClientId(int id);

        Task<List<SentEmail>> GetAllAsync();

        PagedList<SentEmailViewModel> GetByFilter(FilterSentEmailViewModel filter);
        bool EmailAlreadySendedToday(string emailSubject, int? clientId);
    }
}
