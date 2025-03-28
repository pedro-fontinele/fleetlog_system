namespace LOGHouseSystem
{
    public class MelhorEnvioEnvironment
    {
        public string BaseUrl { get; }
        public string ContactEmail { get; }
        public string RedirectUrl { get; }
        public string NotificationEmail { get; }
        public string ClientId { get; set; }
        public string Secret { get; set; }

        public MelhorEnvioEnvironment(IConfigurationSection configurationSection)
        {
            BaseUrl = configurationSection["BaseUrl"];
            RedirectUrl = configurationSection["RedirectUrl"];
            NotificationEmail = configurationSection["NotificationEmail"];
            ContactEmail = configurationSection["ContactEmail"];
            ClientId = configurationSection["ClientId"];
            Secret = configurationSection["Secret"];
        }
    }
}
