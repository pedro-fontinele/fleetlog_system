namespace LOGHouseSystem
{
    public class SmartGoConfigurationEnvironment
    {
        public string Host { get; set; }
        public string ApiKey { get; set; }

        public SmartGoConfigurationEnvironment(IConfigurationSection configurationSection)
        {
            Host = configurationSection["Host"];
            ApiKey = configurationSection["ApiKey"];            
        }
    }
}
