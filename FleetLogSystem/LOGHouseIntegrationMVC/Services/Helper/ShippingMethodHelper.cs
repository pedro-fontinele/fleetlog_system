using LOGHouseSystem.Infra.Enum;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;

namespace LOGHouseSystem.Services.Helper
{
    public static class ShippingMethodHelper
    {
        private static string[] MERCADOLIVRE_MATCHES = new string[]
        {
            "MERCADOLIVRE",
            "MERCADOSHOPSML", //MercadoShopsML
            "MELI"
        };

        private static string[] SHOPEE_MATCHES = new string[]
        {
            "SHOPEE"
        };

        private static string[] MELHORENVIO_MATCHES = new string[]
        {
            "MELHOR ENVIOS",
            "ME"
        };

        private static string[] KANGU_MATCHES = new string[]
        {
            "KANGU"
        };

        private static string[] OLIST_MATCHES = new string[]
        {
            "OLIST"
        };

        private static string[] RaiaDrogasil_MATCHES = new string[]
        {
            "RAIADROGASIL"
        };

        private static string[] MAGALU_MATCHES = new string[]
        {
            "MAGALU",
            "INTEGRACOMMERCE"
        };
        private static string[] AMERICANAS_MATCHES = new string[]
        {
            "AMERICANAS",
            "B2W",
            "SKYHUB"
        };

        private static string[] SHEIN_MATCHES = new string[]
        {
            "SHEIN"
        };

        private static string[] AMAZON_MATCHES = new string[]
        {
            "AMAZONFULFILLMENT",
            "AMAZON"
        };

        public static async Task<ShippingMethodEnum> GetShippingMethod(this string orderShippingMethod, Client client, IIntegrationRepository integrationRepository)
        {
            //if(orderShippingMethod == null) return ShippingMethodEnum.Other; quando nao informado seguir fluxo de associacao baseado na integracao configurada
            if(string.IsNullOrEmpty(orderShippingMethod)) orderShippingMethod = "";

            orderShippingMethod = orderShippingMethod.ToUpper().Trim();
            
            if (SHOPEE_MATCHES.Contains(orderShippingMethod.ToUpper()))
            {
                return ShippingMethodEnum.Shopee;
            }
            else if (MERCADOLIVRE_MATCHES.Contains(orderShippingMethod.ToUpper()))
            {
                return ShippingMethodEnum.MercadoLivre;
            }
            else if (MELHORENVIO_MATCHES.Contains(orderShippingMethod.ToUpper())){
                return ShippingMethodEnum.MelhorEnvio;
            }
            else if (KANGU_MATCHES.Contains(orderShippingMethod.ToUpper()))
            {
                return ShippingMethodEnum.Kangu;
            }
            else if (OLIST_MATCHES.Contains(orderShippingMethod.ToUpper()))
            {
                return ShippingMethodEnum.Olist;
            }
            else if (SHEIN_MATCHES.Contains(orderShippingMethod.ToUpper()))
            {
                return ShippingMethodEnum.Shein;
            }
            else if (RaiaDrogasil_MATCHES.Contains(orderShippingMethod.ToUpper()))
            {
                return ShippingMethodEnum.RaiaDrogasil;
            }
            else if (MAGALU_MATCHES.Contains(orderShippingMethod.ToUpper()))
            {
                return ShippingMethodEnum.Magalu;
            }
            else if (AMAZON_MATCHES.Contains(orderShippingMethod.ToUpper()))
            {
                return ShippingMethodEnum.Amazon;
            }
            else if (AMERICANAS_MATCHES.Contains(orderShippingMethod.ToUpper()))
            {
                return ShippingMethodEnum.Americanas;
            }
            else
            {
                var item = await integrationRepository.GetByClientIdAndNameAsync(client.Id, MelhorEnvioIntegrationNames.IntegrationName);

                if (item != null)
                    return ShippingMethodEnum.MelhorEnvio;

                item = await integrationRepository.GetByClientIdAndNameAsync(client.Id, KanguIntegrationNames.IntegrationName);

                if (item != null)
                    return ShippingMethodEnum.Kangu;

                return ShippingMethodEnum.Other;
            }
        }
    }
}
