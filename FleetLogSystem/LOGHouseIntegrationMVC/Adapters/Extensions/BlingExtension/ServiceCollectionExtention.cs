using LOGHouseSystem.Adapters.Extensions.BlingExtension;

namespace LOGHouseSystem.Adapters.Extensions.BlingExtention
{
    public static class ServiceCollectionExtention
    {
        public static IServiceCollection AddBlingExtension(this IServiceCollection service)
        {
            service.AddScoped<IBlingExtensionService, BlingExtensionService>();

            return service;
        }
    }
}
