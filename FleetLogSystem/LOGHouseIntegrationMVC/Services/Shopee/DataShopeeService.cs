using LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto;
using LOGHouseSystem.Adapters.Extensions.ShopeeExtension;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Adapters.BaseOAUTH2Extension.Dto;
using LOGHouseSystem.Infra.Database;
using System.Globalization;

namespace LOGHouseSystem.Services
{
    public class DataShopeeService : IDataShopeeService
    {
        private IIntegrationRepository _integrationRepository;
        private IIntegrationVariableRepository _integrationVariableRepository;
        private AppDbContext _context;

        public DataShopeeService(IIntegrationRepository integrationRepository, IIntegrationVariableRepository integrationVariableRepository, AppDbContext context)
        {
            _integrationRepository = integrationRepository;
            _integrationVariableRepository = integrationVariableRepository;
            _context = context;
        }

        public async Task<ShopDataShopeeDto> GetShopData(int clientId)
        {
            var integration = await _integrationRepository.GetByClientIdAndNameAsync(clientId, ShopeeIntegrationNames.IntegrationName);
            var integrationVariables = await _integrationVariableRepository.GetByIntegrationIdAsync(integration.Id);

            if (integrationVariables.Count < 4)
            {
                return null;
            }

            var shopId = integrationVariables.Where(e => e.Name == ShopeeIntegrationNames.ShopId).FirstOrDefault();

            if (string.IsNullOrEmpty(shopId?.Value))
            {
                throw new Exception("A configuração está incompleta, é necessário informar o Shop Id da Shopee");
            }

            var data = new ShopDataShopeeDto()
            {
                ClientId = clientId,
                ShopId = Convert.ToInt32(shopId.Value),
                //MainAccountId = Convert.ToInt32(integrationVariables.Where(e => e.Name == ShopeeIntegrationNames.MainAccountId).FirstOrDefault().Value),
            };
            return data;
        }


        public async Task<AuthenticationDto> GetDataAccess(int clientId)
        {
            var integration = await _integrationRepository.GetByClientIdAndNameAsync(clientId, ShopeeIntegrationNames.IntegrationName);

            if (integration == null)
                return null;

            var integrationVariables = await _integrationVariableRepository.GetByIntegrationIdAsync(integration.Id);

            var accessToken = integrationVariables.Where(e => e.Name == ShopeeIntegrationNames.AccessToken).FirstOrDefault();

            if (accessToken == null || accessToken.Value == null)            
                return null;

            return new AuthenticationDto()
            {
                AccessToken = accessToken.Value,
                ClientId = clientId,
                RefreshToken = integrationVariables.Where(e => e.Name == ShopeeIntegrationNames.RefreshToken).FirstOrDefault().Value
            };
        }

        public async Task SetDataAccess(AuthenticationDto newData, int clientId)
        {
            var integration = await _integrationRepository.GetByClientIdAndNameAsync(clientId, ShopeeIntegrationNames.IntegrationName);
            var integrationVariables = await _integrationVariableRepository.GetByIntegrationIdAsync(integration.Id);

            var accessToken = integrationVariables.Where(e => e.Name == ShopeeIntegrationNames.AccessToken).FirstOrDefault();
            var refreshToken = integrationVariables.Where(e => e.Name == ShopeeIntegrationNames.RefreshToken).FirstOrDefault();            

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
                            Name = ShopeeIntegrationNames.AccessToken,
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
                            Name = ShopeeIntegrationNames.RefreshToken,
                            Value = newData.RefreshToken,
                            IntegrationId = integration.Id,
                        };

                        await _integrationVariableRepository.AddAsync(refreshToken);
                    }

                    await transaction.CommitAsync();
                }
                catch(Exception e)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

        }
    }
}
