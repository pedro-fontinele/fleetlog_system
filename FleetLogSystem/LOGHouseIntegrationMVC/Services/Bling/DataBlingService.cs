using LOGHouseSystem.Adapters.BaseOAUTH2Extension.Dto;
using LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto;
using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension.Dto;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Enum;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using System.Globalization;

namespace LOGHouseSystem.Services
{
    public class DataBlingService : IDataBlingService
    {
        private IIntegrationRepository _integrationRepository;
        private IIntegrationVariableRepository _integrationVariableRepository;
        private AppDbContext _context;

        public DataBlingService(IIntegrationRepository integrationRepository, IIntegrationVariableRepository integrationVariableRepository, AppDbContext context)
        {
            _integrationRepository = integrationRepository;
            _integrationVariableRepository = integrationVariableRepository;
            _context = context;
        }

        public async Task<DataBlingV3Dto> GetData(int clientId)
        {
            var integration = await _integrationRepository.GetByClientIdAndNameAsync(clientId, BlingV3IntegrationNames.IntegrationName);
            var integrationVariables = await _integrationVariableRepository.GetByIntegrationIdAsync(integration.Id);

            var clientIdData = integrationVariables.Where(e => e.Name == BlingV3IntegrationNames.BlingClientId).FirstOrDefault().Value;
            var secret = integrationVariables.Where(e => e.Name == BlingV3IntegrationNames.BlingSecret).FirstOrDefault().Value;

            if (string.IsNullOrEmpty(clientIdData))
            {
                throw new Exception("O Client ID do Bling não está cadastrado para esse cliente.");
            }
            else if (string.IsNullOrEmpty(secret))
            {
                throw new Exception("O Secret do Bling não está cadastrado para esse cliente.");
            }

            var data = new DataBlingV3Dto()
            {
                ClientId = clientId,
                BlingClientId = clientIdData,
                BlingClientSecret = secret,
            };

            return data;
        }

        public async Task<AuthenticationDto> GetDataAccess(int clientId)
        {
            var integration = await _integrationRepository.GetByClientIdAndNameAsync(clientId, BlingV3IntegrationNames.IntegrationName);

            if (integration == null)
                throw new Exception($"Dados de conexão do não encontrados para o cliente {clientId}");

            var integrationVariables = await _integrationVariableRepository.GetByIntegrationIdAsync(integration.Id);

            if (integrationVariables.Count < 2)
            {
                return null;
            }

            var accessToken = integrationVariables.Where(e => e.Name == BlingV3IntegrationNames.AccessToken).FirstOrDefault();

            if (accessToken == null || accessToken.Value == null)
                return null;

            return new AuthenticationDto()
            {
                AccessToken = accessToken.Value,
                ClientId = clientId,
                RefreshToken = integrationVariables.Where(e => e.Name == BlingV3IntegrationNames.RefreshToken).FirstOrDefault().Value,
                ExpiresIn = Convert.ToInt32(integrationVariables.Where(e => e.Name == BlingV3IntegrationNames.ExpiresIn).FirstOrDefault().Value)
            };
        }

        public async Task SetDataAccess(AuthenticationDto newData, int clientId)
        {
            var integration = await _integrationRepository.GetByClientIdAndNameAsync(clientId, BlingV3IntegrationNames.IntegrationName);
            var integrationVariables = await _integrationVariableRepository.GetByIntegrationIdAsync(integration.Id);

            var accessToken = integrationVariables.Where(e => e.Name == BlingV3IntegrationNames.AccessToken).FirstOrDefault();
            var refreshToken = integrationVariables.Where(e => e.Name == BlingV3IntegrationNames.RefreshToken).FirstOrDefault();
            var createdAt = integrationVariables.Where(e => e.Name == BlingV3IntegrationNames.AccessCreatedAt).FirstOrDefault();
            var expiresIn = integrationVariables.Where(e => e.Name == BlingV3IntegrationNames.ExpiresIn).FirstOrDefault();

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // access token
                    if (accessToken != null)
                    {
                        accessToken.Value = newData.AccessToken;
                        await _integrationVariableRepository.UpdateAsync(accessToken);
                    }
                    else
                    {
                        accessToken = new IntegrationVariable()
                        {
                            Name = BlingV3IntegrationNames.AccessToken,
                            Value = newData.AccessToken,
                            IntegrationId = integration.Id,
                        };

                        await _integrationVariableRepository.AddAsync(accessToken);
                    }

                    // refresh token
                    if (refreshToken != null)
                    {
                        refreshToken.Value = newData.RefreshToken;
                        await _integrationVariableRepository.UpdateAsync(refreshToken);
                    }
                    else
                    {
                        refreshToken = new IntegrationVariable()
                        {
                            Name = BlingV3IntegrationNames.RefreshToken,
                            Value = newData.RefreshToken,
                            IntegrationId = integration.Id,
                        };

                        await _integrationVariableRepository.AddAsync(refreshToken);
                    }


                    // Expires In
                    if (expiresIn != null)
                    {
                        expiresIn.Value = newData.ExpiresIn.ToString();
                        await _integrationVariableRepository.UpdateAsync(expiresIn);
                    }
                    else
                    {
                        expiresIn = new IntegrationVariable()
                        {
                            Name = BlingV3IntegrationNames.ExpiresIn,
                            Value = newData.ExpiresIn.ToString(),
                            IntegrationId = integration.Id,
                        };

                        await _integrationVariableRepository.AddAsync(expiresIn);
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}
