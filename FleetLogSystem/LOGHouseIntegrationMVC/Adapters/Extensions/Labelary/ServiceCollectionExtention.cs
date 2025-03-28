namespace LOGHouseSystem.Adapters.Extensions.Labelary
{
    public static class ServiceCollectionExtention
    {
        public static string LabelaryBaseURL { get; set; } = "";

        public static IServiceCollection UseLabelaryAPI(this IServiceCollection services, string labelaryBaseURL)
        {
            services.AddScoped<ILabelaryAPIService, LabelaryAPIService>();
            LabelaryBaseURL = labelaryBaseURL;

            return services;
        }
    }
}
