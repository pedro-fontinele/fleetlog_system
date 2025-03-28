using Microsoft.AspNetCore.SignalR;

namespace LOGHouseSystem { 
    public static class Environment
    {
        public static IConfigurationRoot Configuration()
        {
           return new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        }

        public static string DefaultConnection => Configuration().GetConnectionString("DefaultConnection");
        public static string HangfireConnection => Configuration().GetConnectionString("HangfireConnection");
        public static string NFeXmlBasePath => Configuration().GetSection("NFeExtension")["XmlBasePath"];
        public static string PackingPhotoPath => Configuration().GetSection("PackingPhoto")["Path"];
        public static string TransportationPhotoPath => Configuration().GetSection("TransportationPhoto")["Path"];
        public static string DevolutionPhotoPath => Configuration().GetSection("DevolutionPhoto")["Path"];
        public static string TransportationSignaturePath => Configuration().GetSection("TransportationSignature")["Path"];
        public static string TransportationPlatesPath => Configuration().GetSection("TransportationPlatesPath")["Path"];

        //MercadoLivre
        public static string MLAppId => Configuration().GetSection("MercadoLivre")["AppId"];
        public static string MLSecretKey => Configuration().GetSection("MercadoLivre")["SecretKey"];
        public static string MLEndpointUrl => Configuration().GetSection("MercadoLivre")["EndpointUrl"];
        public static string MLAuthBaseUrl => Configuration().GetSection("MercadoLivre")["AuthBaseUrl"];
        public static string MLBaseUrl => Configuration().GetSection("MercadoLivre")["BaseUrl"];
        public static string MLLabelPath => Configuration().GetSection("MercadoLivre")["LabelPath"];
        public static string LogOrderEmail => Configuration().GetSection("Log")["LogOrderEmail"];       
        public static string XmlUploadPath => Configuration().GetSection("XmlUpload")["Path"];       
        public static string XmlCnpjLogHouse => Configuration().GetSection("XmlUpload")["CnpjLogHouse"];       
        public static string TagUploaded => Configuration().GetSection("TagUploaded")["Path"];       
        public static string BlingBaseUrl => Configuration().GetSection("Bling")["BaseUrl"];
        public static string KanguBaseUrl => Configuration().GetSection("Kangu")["BaseUrl"];
        public static string KanguTags => Configuration().GetSection("Kangu")["Tags"];
        public static string BlingV3BaseUrl => Configuration().GetSection("BlingV3")["BaseUrl"];
        public static string ClientIdLogHouse => Configuration().GetSection("BlingV3")["ClientIdLogHouse"];
        public static string BlingV3EmailRejectionNotification => Configuration().GetSection("BlingV3")["EmailRejectionNotification"];
        public static string BlingV3EnvironmentOperationDescription => Configuration().GetSection("BlingV3")["EnvironmentOperationDescription"];        
        public static string DeOlhoNoImpostoToken => Configuration().GetSection("DeOlhoNoImposto")["Token"];

        //URLS
        public static string DiscordWebHookUrl => Configuration().GetSection("Url")["DiscordWebHookUrl"];

        // Shopee
        public static ShopeeEnvironment ShopeeEnvironment => new ShopeeEnvironment(Configuration().GetSection("Shopee"));

        // Melhor envio
        public static MelhorEnvioEnvironment MelhorEnvioEnvironment => new MelhorEnvioEnvironment(Configuration().GetSection("MelhorEnvio"));
        
        // ZPL Configuration
        public static ZplConfigurationEnvironment ZplConfiguration => new ZplConfigurationEnvironment(Configuration().GetSection("ZplConfiguration"));

        // Smart Go
        public static SmartGoConfigurationEnvironment SmartGoConfiguration => new SmartGoConfigurationEnvironment(Configuration().GetSection("SmartGo"));

        public static string EnvironmentName => Configuration()["EnvironmentName"];
    }
}
