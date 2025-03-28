namespace LOGHouseSystem
{
    public class ZplConfigurationEnvironment
    {
        public string BaseSimplifiedDanfe { get; }
        public string LabelaryAPIBaseRoute { get; }
        public string SimplifiedDanfePdfPath { get; }
        public string MargedSimplifiedDanfesPath { get; }

        public ZplConfigurationEnvironment(IConfigurationSection configurationSection)
        {
            BaseSimplifiedDanfe = configurationSection["BaseSimplifiedDanfe"];
            LabelaryAPIBaseRoute = configurationSection["LabelaryAPIBaseRoute"];
            SimplifiedDanfePdfPath = configurationSection["SimplifiedDanfePdfPath"];
            MargedSimplifiedDanfesPath = configurationSection["MargedSimplifiedDanfesPath"];
        }
    }
}
