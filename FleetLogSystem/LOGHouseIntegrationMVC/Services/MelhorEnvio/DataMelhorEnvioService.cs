using LOGHouseSystem.Adapters.BaseOAUTH2Extension.Dto;
using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension;
using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension.Dto;
using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension.Exceptions;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Enum;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using System.Globalization;

namespace LOGHouseSystem.Services
{
    public class DataMelhorEnvioService : IDataMelhorEnvioService
    {
        private IIntegrationRepository _integrationRepository;
        private IIntegrationVariableRepository _integrationVariableRepository;
        private AppDbContext _context;

        public DataMelhorEnvioService(IIntegrationRepository integrationRepository, IIntegrationVariableRepository integrationVariableRepository, AppDbContext context)
        {
            _integrationRepository = integrationRepository;
            _integrationVariableRepository = integrationVariableRepository;
            _context = context;
        }

        public async Task<DataMelhorEnvioDto> GetData(int clientId)
        {
            //var integration = await _integrationRepository.GetByClientIdAndNameAsync(clientId, MelhorEnvioIntegrationNames.IntegrationName);
            //var integrationVariables = await _integrationVariableRepository.GetByIntegrationIdAsync(integration.Id);

            /*var melhorEnvioClientId = integrationVariables.Where(e => e.Name == MelhorEnvioIntegrationNames.MelhorEnvioClientId).FirstOrDefault().Value;
            var melhorEnvioSecret = integrationVariables.Where(e => e.Name == MelhorEnvioIntegrationNames.MelhorEnvioSecret).FirstOrDefault().Value;

            if (string.IsNullOrEmpty(melhorEnvioClientId))
            {
                throw new Exception("O Client ID do Melhor Envio não está cadastrado para esse cliente.");
            }
            else if (string.IsNullOrEmpty(melhorEnvioSecret))
            {
                throw new Exception("O Secret do Melhor Envio não está cadastrado para esse cliente.");
            }*/

            var data = new DataMelhorEnvioDto()
            {
                ClientId = clientId,
                MelhorEnvioClientId = Convert.ToInt32(Environment.MelhorEnvioEnvironment.ClientId),
                MelhorEnvioSecret = Environment.MelhorEnvioEnvironment.Secret,
            };

            return data;
        }

        public async Task<AuthenticationDto> GetDataAccess(int clientId)
        {
            var integration = await _integrationRepository.GetByClientIdAndNameAsync(clientId, MelhorEnvioIntegrationNames.IntegrationName);

            if (integration == null)
                throw new NotFoundDataConnectionMelhorEnvioException($"Dados de conexão do Melhor Envio não encontrados para o cliente {clientId}");

            var integrationVariables = await _integrationVariableRepository.GetByIntegrationIdAsync(integration.Id);

            if (integrationVariables.Count < 2)
            {
                return null;
            }

            var accessToken = integrationVariables.Where(e => e.Name == MelhorEnvioIntegrationNames.AccessToken).FirstOrDefault();

            if (accessToken == null || accessToken.Value == null)
                return null;


            if (accessToken.Value == null)
                return null;

            return new AuthenticationDto()
            {
                AccessToken = accessToken.Value,
                ClientId = clientId,
                RefreshToken = integrationVariables.Where(e => e.Name == MelhorEnvioIntegrationNames.RefreshToken).FirstOrDefault().Value,
                ExpiresIn = Convert.ToInt32(integrationVariables.Where(e => e.Name == MelhorEnvioIntegrationNames.ExpiresIn).FirstOrDefault().Value)
            };
        }

        public async Task SetDataAccess(AuthenticationDto newData, int clientId)
        {
            var integration = await _integrationRepository.GetByClientIdAndNameAsync(clientId, MelhorEnvioIntegrationNames.IntegrationName);
            var integrationVariables = await _integrationVariableRepository.GetByIntegrationIdAsync(integration.Id);

            var accessToken = integrationVariables.Where(e => e.Name == MelhorEnvioIntegrationNames.AccessToken).FirstOrDefault();
            var refreshToken = integrationVariables.Where(e => e.Name == MelhorEnvioIntegrationNames.RefreshToken).FirstOrDefault();
            var createdAt = integrationVariables.Where(e => e.Name == MelhorEnvioIntegrationNames.AccessCreatedAt).FirstOrDefault();
            var expiresIn = integrationVariables.Where(e => e.Name == MelhorEnvioIntegrationNames.ExpiresIn).FirstOrDefault();

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
                            Name = MelhorEnvioIntegrationNames.AccessToken,
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
                            Name = MelhorEnvioIntegrationNames.RefreshToken,
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
                            Name = MelhorEnvioIntegrationNames.ExpiresIn,
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
