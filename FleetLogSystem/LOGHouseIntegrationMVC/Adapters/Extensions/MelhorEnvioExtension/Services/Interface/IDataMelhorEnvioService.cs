using LOGHouseSystem.Adapters.Extensions.BaseOAUTH2Extension.Interface;
using LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension.Dto;

namespace LOGHouseSystem.Adapters.Extensions.MelhorEnvioExtension
{
    /// <summary>
    /// Get Shopee data partner 
    /// </summary>
    /// <returns></returns>
    public interface IDataMelhorEnvioService : IDataAuthenticationService
    {
        Task<DataMelhorEnvioDto> GetData(int clientId);
    }
}