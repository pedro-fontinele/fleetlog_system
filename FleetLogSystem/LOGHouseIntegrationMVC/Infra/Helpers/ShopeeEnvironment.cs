namespace LOGHouseSystem
{
    public class ShopeeEnvironment
    {

        public string BaseUrl { get; }
        public string BaseUrlHom { get; }
        public string RedirectUrl { get; }
        public int PartnerId { get; }
        public string PartnerKey { get; }        
        public string PdfShipping { get; }        
        public string NotificationEmail { get; }        

        public ShopeeEnvironment(IConfigurationSection configurationSection)
        {
            BaseUrl = configurationSection["BaseUrl"];
            BaseUrlHom = configurationSection["BaseUrlHom"];
            RedirectUrl = configurationSection["RedirectUrl"];
            PdfShipping = configurationSection["PdfShipping"];
            NotificationEmail = configurationSection["NotificationEmail:Release"];
            PartnerId = Convert.ToInt32(configurationSection["PartnerId"]);
            PartnerKey = configurationSection["PartnerKey"];
        }
    }
}
