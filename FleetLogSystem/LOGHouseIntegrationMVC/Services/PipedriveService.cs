using LOGHouseSystem.Controllers.API.PipedriveHook.Requests;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services
{
    public class PipedriveService : IPipedriveService
    {
        private readonly IClientsRepository _clientRepository;
        private IClientContractsRepository _clientContractsRepository;
        private IUserRepository _userRepository;
        //private ClientsRepository _clientRepository;
        //private ClientContractsRepository _clientContractsRepository;
        //private UserRepository _userRepository;
        //private AppDbContext _context;

        public PipedriveService(IClientsRepository clientsRepository, IClientContractsRepository clientContractsRepository, IUserRepository userRepository)
        {
            _clientRepository = clientsRepository;
            _clientContractsRepository = clientContractsRepository;
            _userRepository = userRepository;
            //_context = context ?? new AppDbContext();
            //_clientRepository = new ClientsRepository(context);
            //_userRepository = new UserRepository(context);
            //_clientContractsRepository = new ClientContractsRepository(context);
        }


        public void CreateNewClient(PipedriveCreateClientRequest client)
        {
            User user = new User
            {
                Email = client.Email,
                Name = client.RazaoSocial,
                IsActive = Infra.Enums.Status.Ativo,
                PermissionLevel = Infra.Enums.PermissionLevel.Client,
                Username = client.Email,
                FirstAcess = Infra.Enums.YesOrNo.Yes,
                Password = ""
            };

            user = _userRepository.Add(user);

            Client dbClient = new Client
            {
                Adress = client.Endereco,
                Cnpj = client.Cnpj,
                Email = client.Email,
                Phone = client.Telefone,
                SocialReason = client.RazaoSocial,
                UserId = user.Id
            };

            dbClient = _clientRepository.Add(dbClient);

            ClientContract contract = new ClientContract
            {
                ClientId = dbClient.Id,
                ContractValue = client.ValorContrato,
                Requests = client.Pedidos,
                RequestsValue = client.PedidosValor,
                ShippingUnits = client.UnidadesEnvio,
                Storage = client.Armazenagem,
                StorageValue = client.ArmazenagemValor,
                SurplusStorage = client.ArmazenagemExcedente,
            };

            _clientContractsRepository.Add(contract);

        }
    }
}
