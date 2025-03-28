using LOGHouseSystem.Adapters.Extensions.Kangu.Dto;
using RestSharp;

namespace LOGHouseSystem.Adapters.Extensions.Kangu
{
    public interface IKanguExtensionService
    {
        Task<RestResponse> GetTag(string order);
        Task<RestResponse> PostTag(KanguPostTagRequestDto order);
        void SetConnection(string accessToken);
    }
}
