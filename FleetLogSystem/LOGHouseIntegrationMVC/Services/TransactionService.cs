using LOGHouseSystem.Controllers.API.PipedriveHook.Requests;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Services
{
    public class TransactionService
    {
        private AppDbContext _db = new AppDbContext();
        private ClientsRepository _clientRepository;
        private ClientContractsRepository _clientContractRepository;
        private UserRepository _userRepository;
        

        public TransactionService()
        {
            
            _clientRepository = new ClientsRepository();
            _userRepository = new UserRepository();
            _clientContractRepository = new ClientContractsRepository();
        }

        public int CreateNewClientUserAndClientContract(ClientAndYourContractViewModel clientContractViewModel)
        {

            var transaction = _db.Database.BeginTransaction();

            try
            {
                User user = new User()
                {
                    Email = clientContractViewModel.Email,
                    Name = clientContractViewModel.SocialReason,
                    IsActive = Infra.Enums.Status.Ativo,
                    PermissionLevel = Infra.Enums.PermissionLevel.Client,
                    Username = clientContractViewModel.Email,
                    FirstAcess = Infra.Enums.YesOrNo.Yes,
                    Password = ""
                };

                _userRepository.Add(user);

                Client client = new Client()
                {
                    Cnpj = clientContractViewModel.Cnpj,
                    Email = clientContractViewModel.Email,
                    Adress = clientContractViewModel.Adress,
                    SocialReason = clientContractViewModel.SocialReason,
                    Phone = clientContractViewModel.Phone,
                    UserId = user.Id
                };

                _clientRepository.Add(client);

                ClientContract clientContract = new ClientContract()
                {
                    Storage = clientContractViewModel.Storage,
                    SurplusStorage = clientContractViewModel.SurplusStorage,
                    StorageValue = clientContractViewModel.StorageValue,
                    Requests = clientContractViewModel.Requests,
                    RequestsValue = clientContractViewModel.RequestsValue,
                    ShippingUnits = clientContractViewModel.ShippingUnits,
                    ContractValue = clientContractViewModel.ContractValue,
                    InsurancePercentage = clientContractViewModel.InsurancePercentage,
                    ExcessOrderValue = clientContractViewModel.ExcessOrderValue,
                    ClientId = client.Id
                };

                _clientContractRepository.Add(clientContract);

                transaction.Commit();
                return client.Id;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return 0;
            }
        }
    }
}
