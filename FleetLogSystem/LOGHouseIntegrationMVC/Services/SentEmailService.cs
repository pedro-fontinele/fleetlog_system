using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Models.SendEmails;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using System.Text.RegularExpressions;

namespace LOGHouseSystem.Services
{
    public class SentEmailService : ISentEmailService
    {
        private readonly ISentEmailRepository _sentEmailRepository;
        private readonly IClientsRepository _clientsRepository;

        public SentEmailService(ISentEmailRepository sentEmailRepository, IClientsRepository clientsRepository)
        {
            _sentEmailRepository = sentEmailRepository;
            _clientsRepository = clientsRepository;
        }


        public bool AddEmail(EmailData emailData, User? user, int? clientId, int? invoiceNumber = null)
        {
            int? idClient = clientId;

            if(idClient <= 0 || idClient == null)
            {
                Client client = _clientsRepository.GetByUserId(user.Id);
                if (client == null) 
                {
                    return true;
                }
                else
                {
                    idClient = client.Id;
                }
            }

            DateTime currentTime = DateTime.Now;

            // Obtém o fuso horário do Brasil
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

            // Converte a hora atual para o fuso horário do Brasil
            DateTime brazilTime = TimeZoneInfo.ConvertTime(currentTime, TimeZoneInfo.Local, tz);



            SentEmail email = new SentEmail()
            {
                Title = emailData.EmailSubject,
                Body = RemoverTagsHTML(emailData.EmailBody),
                To = emailData.EmailToName,
                ToEmail = emailData.EmailToId,
                SendData = brazilTime,
                ClientId = idClient,
                InvoiceNumber = invoiceNumber
            };

           _sentEmailRepository.Add(email);

            return true; 
        }

        public bool EmailAlreadySendedToday(string emailSubject, int? clientId)
        {
            DateTime today = DateTimeHelper.GetCurrentDateTime();

            return _sentEmailRepository.GetByFilter(new FilterSentEmailViewModel
            {
                ClientId = clientId,
                Title = emailSubject,
                SendData = today
            }).Any();
        }

        public async Task<List<SentEmail>> GetAllAsync()
        {
            return await _sentEmailRepository.GetAllAsync();
        }

        public PagedList<SentEmailViewModel> GetByFilter(FilterSentEmailViewModel filter)
        {
            return _sentEmailRepository.GetByFilter(filter);
        }

        public async Task<List<SentEmail>> GetSentEmailsByClientId(int id)
        {
            return await _sentEmailRepository.GetAllByClientIdAsync(id);
        }
        private string RemoverTagsHTML(string input)
        {
            // Use uma expressão regular para remover as tags HTML e o conteúdo dentro das tags <style>
            string pattern = @"<style\b[^>]*>[\s\S]*?</style>|<.*?>";
            return Regex.Replace(input, pattern, string.Empty);
        }


    }
}
