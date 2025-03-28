using LOGHouseSystem.Adapters.Extensions.TinyExtension.Service;

namespace LOGHouseSystem.Adapters.Extensions.TinyExtension
{
    public static class ServiceCollectionExtention
    {
        public static IServiceCollection AddTinyExtension(this IServiceCollection service)
        {
            service.AddScoped<ITinyAPIService, TinyAPIService>();

            return service;
        }
    }
}
