using LOGHouseSystem.Adapters.Extensions.BaseOAUTH2Extension.Interface;
using LOGHouseSystem.Adapters.Extensions.BlingOAUTHExtension.Dto;

namespace LOGHouseSystem.Services
{
    public interface IDataBlingService : IDataAuthenticationService
    {
        Task<DataBlingV3Dto> GetData(int clientId);
    }
}
