using LOGHouseSystem.Adapters.BaseOAUTH2Extension.Dto;
using LOGHouseSystem.Adapters.Extensions.BaseOAUTH2Extension.Interface;

namespace LOGHouseSystem.Adapters.Extensions.BaseOAUTH2Extension
{
    public abstract class BaseOAUTH2BaseService
    {
        protected int REFRESH_TOKEN_TIME = 30 * 24 * 60 * 60; // days * hours * minutes * seconds
        protected int ACCESS_TOKEN_TIME = 30 * 24 * 60 * 60; // days * hours * minutes * seconds

        private List<AuthenticationDto> _authenticationDto = new List<AuthenticationDto>();
        private IDataAuthenticationService _dataAuthenticationService;

        public string PartnerName = "NAO DEFINIDO";

        public BaseOAUTH2BaseService(IDataAuthenticationService dataAuthenticationService)
        {
            _dataAuthenticationService = dataAuthenticationService;
        }
        /// <summary>
        /// Return access token if it is possible
        /// </summary>
        /// <returns></returns>
        public async Task<AuthenticationDto> GetAccessToken(int clientId, bool tryRefresh = false)
        {
            bool notifyClient = false;

            // take authentication data
            AuthenticationDto autenticationData = await GetOAUTH2AccessData(clientId);

            // verify if authentication from database is valid
            if (autenticationData != null)
            {
                // verify if access token is invalid
                if (tryRefresh)
                {
                    var newAutenticationData = await TakeAccessTokenByRefreshToken(autenticationData.RefreshToken, clientId);

                    if (newAutenticationData != null && newAutenticationData.AccessToken != null)
                    {
                        await SetOAUTH2AccessData(newAutenticationData, clientId);
                        autenticationData = newAutenticationData;
                    }
                    else
                    {
                        notifyClient = true;
                    }
                }
            }
            else
            {
                // if there arn't a token saved, notify client to generate a new token                
                notifyClient = true;
            }

            // if it wasn't possible take access token for any reason 
            if (notifyClient)
            {
                await NotifyClientToTakeAuthorizationCode(clientId);
                return null;
            }
            else
            {
                return autenticationData;
            }
        }


        /// <summary>
        /// Save new access data
        /// </summary>
        /// <param name="newData"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        protected async Task SetOAUTH2AccessData(AuthenticationDto newData, int clientId)
        {
            _authenticationDto.RemoveAll(e => e.ClientId == clientId);

            if (newData != null && newData.AccessToken != null)
            {
                await _dataAuthenticationService.SetDataAccess(newData, clientId);
            }            

            _authenticationDto.Add(newData);
        }


        /// <summary>
        /// Convert OAUTH2 Shopee return to our Dto
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        protected AuthenticationDto ConvertResponseToDto(AuthenticationAPIResponseDto accessToken, int clientId)
        {
            return new AuthenticationDto()
            {
                AccessToken = accessToken.AccessToken,
                RefreshToken = accessToken.RefreshToken,
                ExpiresIn = accessToken.ExpiresIn,
                ClientId = clientId
            };
        }


        /// <summary>
        /// Send to user system that needs the URL to take a new authorization code that is necessary to take the JWT access token
        /// </summary>
        /// <returns></returns>
        protected async Task NotifyClientToTakeAuthorizationCode(int clientId)
        {
            string url = await GenerateAuthorizationCodeUrl(clientId);

            await RequestUserAuthorizationCode(url, clientId);

            throw new AuthenticationNotFoundException("Token de acesso não encontrado ou expirado! O usuário foi notificado para realizar a autorização da requisição.");
        }


        private async Task<AuthenticationDto> GetOAUTH2AccessData(int clientId)
        {
            var data = _authenticationDto.Where(e => e.ClientId == clientId).FirstOrDefault();

            if (data == null)
            {
                data = await _dataAuthenticationService.GetDataAccess(clientId);
            }

            if (data != null)
            {
                _authenticationDto.Add(data);
            }

            return data;
        }


        /// <summary>
        /// Use the authorization code to take the access token
        /// </summary>
        /// <param name="authorizationCode"></param>
        /// <returns></returns>
        public async Task GetAccessNewToken(string authorizationCode, int clientId)
        {
            var accessToken = await GetAccessTokenByCode(authorizationCode, clientId);

            if (accessToken != null && accessToken.AccessToken != null)
            {
                var accessTokenDto = ConvertResponseToDto(accessToken, clientId);
                await SetOAUTH2AccessData(accessTokenDto, clientId);
            }
        }

        /// <summary>
        /// Get Access Token By Code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public abstract Task<AuthenticationAPIResponseDto> GetAccessTokenByCode(string code, int clientId);

        /// <summary>
        /// Implemented by extension
        /// Function responsable to generate and return URL
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public abstract Task<string> GenerateAuthorizationCodeUrl(int clientId);


        /// <summary>
        /// Implemented by client
        /// </summary>
        /// <param name="url"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        protected abstract Task RequestUserAuthorizationCode(string url, int clientId);

        /// <summary>
        /// Implemented by extension
        /// Function responsable for refresh the access token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        protected abstract Task<AuthenticationDto> TakeAccessTokenByRefreshToken(string refreshToken, int clientId);
    }
}
