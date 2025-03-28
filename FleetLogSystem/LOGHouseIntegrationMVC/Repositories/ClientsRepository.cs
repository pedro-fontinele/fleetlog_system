using Hangfire.Storage;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Web.Helpers;

namespace LOGHouseSystem.Repositories
{
    public class ClientsRepository : RepositoryBase, IClientsRepository
    {
        private SessionHelper _session = new SessionHelper();


        public List<Client> GetAll()
        {
           return _db.Clients
                .AsNoTracking()
                .ToList();
        }

        public object GetByName(string prefix)
        {

            var clients = (from client in _db.Clients
                           where client.SocialReason.StartsWith(prefix)
                           select new
                           {
                               label = client.SocialReason,
                               val = client.Id,
                               vald = client.Cnpj
                           }).ToList();


            return (clients);
        }

        public object GetByClientId()
        {

            Client clientByUser = GetByUserLoged();


            var clients = (from client in _db.Clients
                           where client.Id == clientByUser.Id
                           select new
                           {
                               label = client.SocialReason,
                               val = client.Id, 
                               vald = client.Cnpj
                           }).ToList();


            return (clients);
        }


        public Client GetById(int? id)
        {
            return _db.Clients.Include(x => x.User).FirstOrDefault(x => x.Id == id);
        }

        public Client GetByUserLoged()
        {
            User user = _session.SearchUserSession();

            if (user == null || user.Id <= 0) return null;

            return _db.Clients
                      .Where(x => x.UserId == user.Id)
                      .Include(x => x.User)
                      .FirstOrDefault();

        }

        public ClientContract GetContract(int id)
        {
            return _db.ClientContracts.FirstOrDefault(x => x.ClientId == id);
        }

        public Client Add(Client client)
        {
            _db.Clients.Add(client);
            _db.SaveChanges();

            return client;
        }

        public Client FindByCnpj(string cnpj)
        {
            return _db.Clients.Where(x => x.Cnpj.Contains(cnpj))
                .AsNoTracking()
                .FirstOrDefault();
        }


        public Task<Client> FindByCnpjAsync(string cnpj)
        {
            return _db.Clients.Where(x => x.Cnpj.Contains(cnpj))
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public Client SearchByCnpj(string cnpj, int? id)
        {
            var query = _db.Clients
                         .Where(x => x.Cnpj == cnpj);

            if (id > 0)
            {
                //Traz se o id for diferente do dele
                query = query.Where(x => x.Id != id);
            }

            return query.FirstOrDefault();
        }


        public Client SearchByEmail(string email, int? id)
        {
            var query = _db.Clients
                           .Where(x => x.Email == email);

            if (id > 0)
            {
                query = query.Where(x => x.Id != id);
            }

            return query.FirstOrDefault();
        }

        public bool CheckClient(string? cnpj, string? email, int? id)
        {

            if(cnpj != null)
            {
                Client userByCnpj = SearchByCnpj(cnpj, id);

                if (userByCnpj != null)
                {
                    return false;
                }
            }

            if(email != null)
            {
                Client userByLogin = SearchByEmail(email, id);

                if (userByLogin != null)
                {
                    return false;
                }
            }

            return true;
        }

        public ClientViewModel UpdateClient(ClientViewModel client)
        {
            var clientById = GetById(client.Id);

            if (client == null)
                throw new System.Exception("Houve um erro na atualização do cliente");

            clientById.Cnpj = client.Cnpj;
            clientById.Email = client.Email;
            clientById.Adress = client.Adress;
            clientById.SocialReason = client.SocialReason;
            clientById.Phone = client.Phone;
            clientById.StateRegistration = client.StateRegistration;

            _db.Clients.Update(clientById);
            _db.SaveChanges();

            return client;
        }

        public ClientAndYourContractViewModel Update(ClientAndYourContractViewModel client)
        {
            var clientById = GetById(client.ClientId);
            var contractById = GetContract(clientById.Id);

            if (client == null)
                throw new System.Exception("Houve um erro na atualização do cliente");

            if (contractById == null)
                throw new System.Exception("Houve um erro na atualização do cliente");

            clientById.Cnpj = client.Cnpj;
            clientById.Email = client.Email;
            clientById.Adress = client.Adress;
            clientById.SocialReason = client.SocialReason;
            clientById.Phone = client.Phone;
            clientById.StateRegistration = client.StateRegistration;

            if (client.Storage != null)
            {
                contractById.Storage = client.Storage;
                contractById.SurplusStorage = client.SurplusStorage;
                contractById.StorageValue = client.StorageValue;
                contractById.Requests = client.Requests;
                contractById.RequestsValue = client.RequestsValue;
                contractById.ShippingUnits = client.ShippingUnits;
                contractById.ContractValue = client.ContractValue;
                contractById.ExcessOrderValue = client.ExcessOrderValue;
                contractById.InsurancePercentage = client.InsurancePercentage;
            }
           
            _db.Clients.Update(clientById);
            _db.ClientContracts.Update(contractById);
            _db.SaveChanges();

            return client;
        }

        public async Task<Client> GetByIdAsync(int id)
        {
            return await _db.Clients.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public Client GetByUserId(int id)
        {
            return _db.Clients
                       .Where(x => x.UserId == id)
                       .FirstOrDefault();
        }

        public object GetIfContainsPrefix(string prefix)
        {
            var clients = (from client in _db.Clients
                           where client.SocialReason.Contains(prefix)
                           select new
                           {
                               label = client.SocialReason,
                               val = client.Id,
                               vald = client.Cnpj
                           }).ToList();


            return (clients);
        }

        public Client UpdateStatus(Client client, YesOrNo status)
        {
            client.IsActive = status;

            _db.Clients.Update(client);
            _db.SaveChanges();

            return client;
        }
    }
}
