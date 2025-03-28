using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension;
using LOGHouseSystem.Models.SendEmails;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services
{
    public class ClientMelhorEnvioService : MelhorEnvioAPIServices
    {
        private IClientsRepository _clientsRepository;
        private IEmailService _emailService;

        public ClientMelhorEnvioService(IDataMelhorEnvioService dataMelhorEnvioService,
            IClientsRepository clientsRepository,
            IEmailService emailService) : base(dataMelhorEnvioService)
        {
            _clientsRepository = clientsRepository;
            _emailService = emailService;
        }

        public override string GetAuthorizationRedirectUrl() 
        {            
            return Environment.MelhorEnvioEnvironment.RedirectUrl;
        }

        protected override string GetContactEmail()
        {
            return Environment.MelhorEnvioEnvironment.ContactEmail;
        }

        protected override async Task RequestUserAuthorizationCode(string url, int clientId)
        {
            var client = await _clientsRepository.GetByIdAsync(clientId);

            _emailService.SendEmail(new EmailData
            {
                EmailBody = $@"Integração com o Melhor Envio requisitando autenticação para o cliente {client.SocialReason} - {client.Cnpj}, pare realizar, <a href='{url}'>Clique Aqui</a>.",
                EmailSubject = $"Melhor Envio - Autenticação necessária {client.SocialReason} - {client.Cnpj}",
                EmailToId = Environment.MelhorEnvioEnvironment.NotificationEmail,
                EmailToName = Environment.MelhorEnvioEnvironment.NotificationEmail
            }, null, client.Id);
        }
    }
}
