using LOGHouseSystem.Adapters.Extensions.ShopeeExtension;
using LOGHouseSystem.Models;
using LOGHouseSystem.Models.SendEmails;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services
{
    public class ClientShopeeService : APIShopeeService
    {        
        private IEmailService _emailService;
        private IClientsRepository _clientsRepository;
        private IUserRepository _userRepository;

        public ClientShopeeService(IEmailService emailService, IClientsRepository clientsRepository, IDataShopeeService dataShopeeService, IUserRepository userRepository) : base(dataShopeeService)
        {            
            _emailService = emailService;
            _clientsRepository = clientsRepository;
            _userRepository = userRepository;
        }

        public override string GetAuthorizationRedirectUrl()
        {            
            return Environment.ShopeeEnvironment.RedirectUrl;
        }

        public override string GetHostUrl()
        {
            var builder = WebApplication.CreateBuilder();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                return Environment.ShopeeEnvironment.BaseUrl;
            }
            else
            {
                return Environment.ShopeeEnvironment.BaseUrlHom;                
            }
            
        }

        protected override async Task RequestUserAuthorizationCode(string url, int clientId)
        {
            var client = await _clientsRepository.GetByIdAsync(clientId);

            _emailService.SendEmail(new EmailData
            {
                EmailBody = $@"Integração com a Shopee requisitando autenticação para o cliente {client.SocialReason} - {client.Cnpj}, pare realizar, click no Link: {url}.",
                EmailSubject = $"Shopee - Autenticação necessária {client.SocialReason} - {client.Cnpj}",
                EmailToId = Environment.ShopeeEnvironment.NotificationEmail,
                EmailToName = Environment.ShopeeEnvironment.NotificationEmail
            }, null, client.Id);
        }
    }
}
