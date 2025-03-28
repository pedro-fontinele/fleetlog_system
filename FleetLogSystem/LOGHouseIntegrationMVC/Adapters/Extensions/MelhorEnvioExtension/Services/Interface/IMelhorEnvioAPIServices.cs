using LOGHouseSystem.Adapters.BaseOAUTH2Extension.Dto;
using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension.Dto.Response;

namespace LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension
{
    public interface IMelhorEnvioAPIServices
    {
        Task GetAccessNewToken(string authorizationCode, int clientId);
        Task<GetShippingDataMelhorEnvioDtoResponse> GetShippingData(List<string> order, int clientId, bool secondTrying = false);
        Task<GetAllShippingMelhorEnvioDtoResponse> GetAllShippingByState(string state, int clientId, bool secondTrying = false);
        Task<GetTagMelhorEnvioResponse> GetTagInformation(string tag, int clientId, bool secondTrying = false);
        Task<string> GenerateAuthorizationCodeUrl(int clientId);
        string GetBaseUrlAuthorizationCode(int clientId);
        Task<GetAllShippingMelhorEnvioDtoResponse> GetAllShippingByName(string? name, int clientId, bool secondTrying = false);
        Task<AuthenticationDto> GetAccessToken(int clientId, bool tryRefresh = false);
    }
}
