using LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension;
using LOGHouseSystem.Infra.Enum;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Integrations;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services
{
    public class IntegrationService : IIntegrationService
    {
        private const string MERCADO_LIVRE = "MERCADO LIVRE";
        private const string MELHOR_ENVIO = "MELHOR ENVIO";

        private const string TINY = TinyIntegationNames.IntegrationName;
        private const string BLING = BlingIntegrationNames.IntegrationName;
        private const string BLINGV3 = BlingV3IntegrationNames.IntegrationName;
        private const string SHOPEE = ShopeeIntegrationNames.IntegrationName;
        private const string KANGU = KanguIntegrationNames.IntegrationName;


        private List<string> acceptedMarketplaceNames;

        private List<string> acceptedERPNames;


        private IIntegrationRepository _integrationRepository { get; set; }
        private IIntegrationVariableRepository _integrationVariableRepository { get; set; }

        private IIntegrationMercadoLivreService _integrationMercadoLivreService;
        private ITinyHookService _tinyHookService;
        private IBlingCallbackService _blingCallbackService;
        private IMelhorEnvioService _melhorEnvioService;
        private IClientsRepository _clientRepository;
        private IAPIBlingService _iAPIBlingService;
        private IShopeeService _shopeeService;
        private IKanguService _kanguService;

        public IntegrationService(IIntegrationRepository integrationRepository, 
             IIntegrationVariableRepository integrationVariableRepository, 
             IIntegrationMercadoLivreService integrationMercadoLivreService,
             ITinyHookService tinyHookService,
             IBlingCallbackService blingCallbackService,
             IAPIBlingService iAPIBlingService,
             IMelhorEnvioService melhorEnvioService,
             IClientsRepository clientsRepository,
             IShopeeService shopeeService,
             IKanguService kanguService) {
            _integrationRepository = integrationRepository;
            _integrationVariableRepository = integrationVariableRepository;
            _integrationMercadoLivreService = integrationMercadoLivreService;
            _tinyHookService = tinyHookService;
            _blingCallbackService = blingCallbackService;
            _melhorEnvioService = melhorEnvioService;
            _clientRepository = clientsRepository;
            _iAPIBlingService = iAPIBlingService;
            _shopeeService = shopeeService;
            _kanguService = kanguService;

            acceptedMarketplaceNames = new List<string>
            {
                MERCADO_LIVRE,
                SHOPEE,
                MELHOR_ENVIO,
                KANGU
            };

            acceptedERPNames = new List<string> 
            { 
                BLING,
                BLINGV3,
                TINY 
            };
        }

        public Integration CreateNewIntegration(Integration integration)
        {
            if ((integration.Type == Infra.Enums.IntegrationType.ERP && !acceptedERPNames.Contains(integration.Name)) || (integration.Type == Infra.Enums.IntegrationType.Marketplace && !acceptedMarketplaceNames.Contains(integration.Name)))
                throw new Exception("Integração inválida");

            integration = _integrationRepository.Add(integration);

            CreateIntegrationVariables(integration);

            return integration;
        }

        private void CreateIntegrationVariables(Integration integration)
        {
            switch (integration.Name)
            {
                case MERCADO_LIVRE:
                    _integrationMercadoLivreService.CreateIntegrationVariables(integration.Id);
                    break;
                case TINY:
                    _tinyHookService.CreateIntegrationVariables(integration.Id);
                    break;
                case BLING:
                    _blingCallbackService.CreateIntegrationVariables(integration.Id);
                    break;
                case BLINGV3:
                    _iAPIBlingService.CreateV3IntegrationVariables(integration.Id, integration.Client.Id);
                    break;
                case MELHOR_ENVIO:
                    _melhorEnvioService.CreateIntegrationVariables(integration.Id, integration.Client.Id);
                    break;
                case SHOPEE:
                    _shopeeService.CreateIntegrationVariables(integration.Id, integration.Client.Id);
                    break;
                case KANGU:
                    _kanguService.CreateIntegrationVariables(integration.Id);
                    break;
            }
        }

        public async Task<bool> CheckIfIntegrationAlreadyExist(Integration integration)
        {
            Client client = _clientRepository.GetByUserLoged();
            if (client == null)
                throw new Exception("Não foi possível encontrar o cliente, por favor, reinicie a aplicação");

            Integration integrationByNmae = await _integrationRepository.GetByClientIdAndNameAsync(client.Id, integration.Name);

            if (integrationByNmae == null)
                return false;

            return true;
        }

        public async Task DeleteIntegration(int id)
        {
            await _integrationVariableRepository.DeleteAllByIntegrationId(id);
            await _integrationRepository.DeleteById(id);
        }
    }
}
