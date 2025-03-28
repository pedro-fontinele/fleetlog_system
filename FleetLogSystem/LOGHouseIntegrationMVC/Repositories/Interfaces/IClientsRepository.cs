using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IClientsRepository
    {
        List<Client> GetAll();

        Client GetById(int? id);

        Client GetByUserLoged();

        ClientContract GetContract(int id);

        Client Add(Client client);

        Client UpdateStatus(Client client, YesOrNo status);

        Client FindByCnpj(string cnpj);

        Client SearchByCnpj(string cnpj, int? id);

        Client GetByUserId(int id);


        Client SearchByEmail(string email, int? id);

        bool CheckClient(string? cnpj, string? email, int? id);

        object GetByName(string prefix);
        object GetIfContainsPrefix(string prefix);
        object GetByClientId();

        ClientViewModel UpdateClient(ClientViewModel client);

        ClientAndYourContractViewModel Update(ClientAndYourContractViewModel client);
        Task<Client> FindByCnpjAsync(string cnpj);
        Task<Client> GetByIdAsync(int id);       
    }
}
