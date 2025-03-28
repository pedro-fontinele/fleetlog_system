using LOGHouseSystem.Adapters.BaseOAUTH2Extension.Dto;

namespace LOGHouseSystem.Adapters.Extensions.BaseOAUTH2Extension.Interface
{
    public interface IDataAuthenticationService
    {
        Task SetDataAccess(AuthenticationDto newData, int clientId);
        Task<AuthenticationDto> GetDataAccess(int clientId);
    }
}
