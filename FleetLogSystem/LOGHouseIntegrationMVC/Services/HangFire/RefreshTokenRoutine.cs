using LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension;
using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension;
using LOGHouseSystem.Adapters.Extensions.ShopeeExtension;
using LOGHouseSystem.Infra.Enum;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Integrations;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services.HangFire
{
    public class RefreshTokenRoutine : IRefreshTokenRoutine
    {
        private IIntegrationRepository _integrationRespository;
        private IAPIBlingService _iAPIBlingService;
        private IMelhorEnvioAPIServices _melhorEnvioAPIServices;
        private IAPIShopeeService _iAPIShopeeService;
        private IIntegrationMercadoLivreService _integrationMercadoLivreService;

        public RefreshTokenRoutine(IIntegrationRepository integrationRespository, IAPIBlingService iAPIBlingService, IMelhorEnvioAPIServices melhorEnvioAPIServices, IAPIShopeeService iAPIShopeeService, IIntegrationMercadoLivreService integrationMercadoLivreService)
        {
            _integrationRespository = integrationRespository;
            _iAPIBlingService = iAPIBlingService;
            _melhorEnvioAPIServices = melhorEnvioAPIServices;
            _iAPIShopeeService = iAPIShopeeService;
            _integrationMercadoLivreService = integrationMercadoLivreService;
        }
        public async Task RefreshAccessTokensRoutine()
        {
            if (Environment.EnvironmentName == "Development") return;

            List<Integration> integrations  = await _integrationRespository.GetAllIntegrationsByName(new List<string>()
            {
                BlingV3IntegrationNames.IntegrationName,
                MelhorEnvioIntegrationNames.IntegrationName,
                ShopeeIntegrationNames.IntegrationName,
                IntegrationMercadoLivreService.IntegrationVariableName
            });


            foreach (var integration in integrations)
            {
                try
                {
                    switch (integration.Name)
                    {
                        case BlingV3IntegrationNames.IntegrationName:
                            await _iAPIBlingService.GetAccessToken(Convert.ToInt32(integration.ClientId), true);
                            break;
                        case MelhorEnvioIntegrationNames.IntegrationName:
                            await _melhorEnvioAPIServices.GetAccessToken(Convert.ToInt32(integration.ClientId), true);
                            break;
                        case ShopeeIntegrationNames.IntegrationName:
                            await _iAPIShopeeService.GetAccessToken(Convert.ToInt32(integration.ClientId), true);
                            break;
                        case IntegrationMercadoLivreService.IntegrationVariableName:
                            await _integrationMercadoLivreService.SaveRefreshToken(Convert.ToInt32(integration.ClientId));
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format(ex.Message));

                }
            }
        }
    }
}
