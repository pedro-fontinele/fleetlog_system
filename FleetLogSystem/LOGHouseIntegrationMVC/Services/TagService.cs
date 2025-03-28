using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Services.Integrations;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services
{
    public class TagService : ITagService
    {
        private IIntegrationMercadoLivreService _integrationMercadoLivreService;
        private IMelhorEnvioService _melhorEnvioService;
        private IShopeeService _shopeeService;        
        private readonly IKanguService _kanguService;

        public TagService(IMelhorEnvioService melhorEnvioService,
            IShopeeService shopeeService,
            IMelhorEnvioAPIServices melhorEnvioAPIServices,
            IKanguService kanguService,
            IIntegrationMercadoLivreService integrationMercadoLivreService
            )
        {
            _integrationMercadoLivreService = integrationMercadoLivreService;
            _melhorEnvioService = melhorEnvioService;
            _shopeeService = shopeeService;
            
            _kanguService = kanguService;
        }

        public async Task ProcessMarketplaceTag(ExpeditionOrder order)
        {
            switch (order?.ShippingMethod)
            {
                case ShippingMethodEnum.MercadoLivre:
                    await _integrationMercadoLivreService.DownlodShippment(order);
                    break;
                case ShippingMethodEnum.Shopee:
                    await _shopeeService.DownloadPdfShippingFileByExpeditionOrder(order);
                    break;
                case ShippingMethodEnum.MelhorEnvio:
                    await _melhorEnvioService.DownloadShippment(order);
                    break;
                case ShippingMethodEnum.Kangu:
                    await _kanguService.DownloadShippment(order);
                    break;
            }
        }
    }
}
