using LOGHouseSystem.Adapters.API.MercadoLivre;
using LOGHouseSystem.Adapters.API.MercadoLivre.Exceptions;
using LOGHouseSystem.Adapters.API.MercadoLivre.Response;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net;

namespace LOGHouseSystem.Services.Integrations
{
    public enum VariableNames
    {
        URL_AUTH,
        ML_USERID,
        ML_CODE,
        REFRESH_TOKEN,
        ACCESS_TOKEN
    }    

    public class IntegrationMercadoLivreService : IIntegrationMercadoLivreService
    {
        public const string IntegrationVariableName = "MERCADO LIVRE";

        private IIntegrationVariableRepository _integrationVariableRepository { get; set; }
        private IIntegrationRepository _integrationRepository { get; set; }

        private IMercadoLivreExtension _mercadoLivreExtension;
        private IExpeditionOrderTagShippingRepository _expeditionOrderShippingRepository;
        private IClientsRepository _clientsRepository;

        public List<string> IntegrationVariableNames = Enum.GetNames(typeof(VariableNames)).ToList();

        public IntegrationMercadoLivreService(IIntegrationRepository integrationRepository, 
            IIntegrationVariableRepository integrationVariableRepository, 
            IMercadoLivreExtension mercadoLivreExtension,
            IExpeditionOrderTagShippingRepository expeditionOrderShippingRepository,
            IClientsRepository clientsRepository)
        {
            _integrationVariableRepository = integrationVariableRepository;
            _integrationRepository = integrationRepository;
            _mercadoLivreExtension = mercadoLivreExtension;
            _expeditionOrderShippingRepository = expeditionOrderShippingRepository;
            _clientsRepository = clientsRepository;
        }
        
        public List<IntegrationVariable> CreateIntegrationVariables(int integrationId)
        {
            MercadoLivreExtension mercadoLivreExtension = new MercadoLivreExtension();
            List <IntegrationVariable> integrationVariables = new List <IntegrationVariable>();

            foreach (string variable in IntegrationVariableNames)
            {
                string value = variable == VariableNames.URL_AUTH.ToString() ? mercadoLivreExtension.GetAuthenticationUrl(integrationId.ToString()) : "";

                integrationVariables.Add(new IntegrationVariable
                {
                    IntegrationId = integrationId,
                    Name = variable,
                    Value = value
                });
            }

            _integrationVariableRepository.AddRange(integrationVariables);

            return integrationVariables;
        }

        public async Task ConfigMLAccessToken(int integrationId, string code)
        {
            MercadoLivreExtension mercadoLivreExtension = new MercadoLivreExtension();

            var result = await mercadoLivreExtension.GetAccessToken(code);
            UpdateIntegrationVariable(integrationId, VariableNames.ACCESS_TOKEN.ToString(), result.access_token);
            UpdateIntegrationVariable(integrationId, VariableNames.REFRESH_TOKEN.ToString(), result.refresh_token);
            UpdateIntegrationVariable(integrationId, VariableNames.ML_USERID.ToString(), $"{result.user_id}");
            UpdateIntegrationVariable(integrationId, VariableNames.ML_CODE.ToString(), code);
        }

        public async Task UpdateMLAccessTokenByRefreshToken(int integrationId, GetAccessTokenResponse result)
        {        
            UpdateIntegrationVariable(integrationId, VariableNames.ACCESS_TOKEN.ToString(), result.access_token);
            UpdateIntegrationVariable(integrationId, VariableNames.REFRESH_TOKEN.ToString(), result.refresh_token);
            UpdateIntegrationVariable(integrationId, VariableNames.ML_USERID.ToString(), $"{result.user_id}");            
        }

        private IntegrationVariable UpdateIntegrationVariable(int integrationId, string variableName, string value)
        {
            IntegrationVariable integrationVariable = _integrationVariableRepository.GetByIntegrationIdAndName(integrationId, variableName);
            integrationVariable.Value = value;
            integrationVariable = _integrationVariableRepository.Update(integrationVariable);

            return integrationVariable;
        }

        public async Task SetClientData(int clientId)
        {
            var client = await _clientsRepository.GetByIdAsync(clientId);

            var integrationsData = await _integrationRepository.GetByClientIdAndNameAsync(clientId, IntegrationVariableName);

            if (integrationsData == null)
                throw new NotFoundDataConnectionMercadoLivreException($"Dados de conexão do Mercado Livre do cliente {client.SocialReason} não estão cadastrados.");

            var integrationsVariables = await _integrationVariableRepository.GetByIntegrationIdAsync(integrationsData.Id);

            if (integrationsVariables.Count < 3)            
                throw new NotFoundDataConnectionMercadoLivreException($"Dados de conexão do Mercado Livre do cliente {client.SocialReason} não estão cadastrados.");
            
            var userId = integrationsVariables.Where(e => e.Name == VariableNames.ML_USERID.ToString()).FirstOrDefault().Value;

            if(string.IsNullOrEmpty(userId))
                throw new NotFoundDataConnectionMercadoLivreException($"User Id do Mercado Livre do cliente {client.SocialReason} não estão cadastrados.");

            var accessToken = integrationsVariables.Where(e => e.Name == VariableNames.ACCESS_TOKEN.ToString()).FirstOrDefault().Value;

            _mercadoLivreExtension.SetMercadoLivreConnection(
                accessToken,
                Convert.ToInt32(userId));
        }

        public async Task<(string, int)> GetRefreshToken(int clientId)
        {
            var client = await _clientsRepository.GetByIdAsync(clientId);

            var integrationsData = await _integrationRepository.GetByClientIdAndNameAsync(clientId, IntegrationVariableName);

            var integrationsVariables = await _integrationVariableRepository.GetByIntegrationIdAndNameAsync(integrationsData.Id, VariableNames.REFRESH_TOKEN.ToString());

            if(integrationsVariables == null)
                throw new NotFoundDataConnectionMercadoLivreException($"Dados de refresh token do Mercado Livre do cliente {client.SocialReason} não estão cadastrado.");

            return (integrationsVariables.Value, integrationsData.Id);
        }


        public async Task DownlodShippment(ExpeditionOrder order, bool secondTrying = false)
        {
            var mlOrderId = order.ShippingMethodCodeOrder;

            string pathZip = string.Format("{0}/{1}-{2}-TAG-temp.zip", Environment.MLLabelPath, order.Id, mlOrderId);

            string path = string.Format("{0}/{1}-{2}-TAG.zpl", Environment.MLLabelPath, order.Id, mlOrderId);

            if (File.Exists(path))
            {
                return;
            }            

            await SetClientData(Convert.ToInt32(order.ClientId));

            var response = await _mercadoLivreExtension.GetShippingId(mlOrderId);

            if (await ThereIsAuthenticationError(response, Convert.ToInt32(order.ClientId), secondTrying))
            {
                await DownlodShippment(order, true);
                return;
            }

            OrderDetailsResponse mlOrderData = JsonConvert.DeserializeObject<OrderDetailsResponse>(response.Content);

            if (mlOrderData.Results.Count < 0)
                throw new Exception($"Pedido {mlOrderId} não encontrado par o cliente {order.ClientId}");

            RestResponse shippment = await _mercadoLivreExtension.GetLabel(mlOrderData.Results[0].Shipping.Id.ToString());            

            if (await ThereIsAuthenticationError(shippment, Convert.ToInt32(order.ClientId), secondTrying))
            {
                await DownlodShippment(order, true);
                return;
            }

            if (!shippment.IsSuccessStatusCode)
            {
                var shippmentDto = JsonConvert.DeserializeObject<DownloadShippmentTagMercadoLivreResponeDto>(shippment.Content);

                throw new Exception($"Não foi possível baixar a etiqueta do pedido {mlOrderId} do Mercado livre. Mensagem: {shippmentDto.ErrorCode} - {shippmentDto.Message}");
            }                

            byte[] bytes = shippment.RawBytes;            

            File.WriteAllBytes(pathZip, bytes);


            using (ZipArchive zip = ZipFile.OpenRead(pathZip))
            {
                foreach (ZipArchiveEntry entry in zip.Entries.Where(e => e.FullName.Contains("Etiqueta de envio.txt")))
                {
                    entry.ExtractToFile(path);
                }
            }

            File.Delete(pathZip);

            var orderShipping = new ExpeditionOrderTagShipping()
            {
                InvoiceAccessKey = order.InvoiceAccessKey,
                OrderTagOrigin = ShippingMethodEnum.MercadoLivre,
                ExpeditionOrderId = order.Id,
                ShippingCode = mlOrderData.Results[0].Shipping.Id.ToString(),
                Url = path,
                FileFormat = FileFormatEnum.Zpl
            };

            Log.Info($"Inserindo {order.Id} do cliente {order.ClientId}");
            await _expeditionOrderShippingRepository.AddAsync(orderShipping);
        }

        /// <summary>
        /// This function is used to take new access tokens by refresh tokens when the request is unauthorized
        /// </summary>
        /// <param name="response"></param>
        /// <param name="clientId"></param>
        /// <param name="secondTrying"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundDataConnectionMercadoLivreException"></exception>
        private async Task<bool> ThereIsAuthenticationError(RestResponse response, int clientId, bool secondTrying)
        {
            if (response?.StatusCode == HttpStatusCode.Unauthorized)
            {
                if(secondTrying)
                    throw new NotFoundDataConnectionMercadoLivreException($"Não foi possivel requisitar uma conexão com o Mercado Livre do cliente {clientId} utilizando os dados cadastrados.");

                return await SaveRefreshToken(clientId);
            }

            return false;
        }

        public async Task<bool> SaveRefreshToken(int clientId)
        {
            (var refreshToken, int integrationId) = await GetRefreshToken(clientId);

            var responseToken = await _mercadoLivreExtension.RefreshAccessToken(refreshToken);

            if (!responseToken.IsSuccessStatusCode)
            {
                throw new NotFoundDataConnectionMercadoLivreException($"Não foi possivel requisitar uma conexão com o Mercado Livre do cliente {clientId} utilizando os dados cadastrados.");
            }
            else
            {
                GetAccessTokenResponse tokens = JsonConvert.DeserializeObject<GetAccessTokenResponse>(responseToken.Content);
                UpdateMLAccessTokenByRefreshToken(integrationId, tokens);
                return true;
            }
        }
    }
}
