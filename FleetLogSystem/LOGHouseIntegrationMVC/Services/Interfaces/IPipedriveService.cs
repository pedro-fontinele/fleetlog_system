using LOGHouseSystem.Controllers.API.PipedriveHook.Requests;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IPipedriveService
    {
        void CreateNewClient(PipedriveCreateClientRequest client);
    }
}
