using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.ViewModels
{
    public class PackingAreaViewModel
    {
        public string InvoiceAccessKey { get; set; }
        public bool Cart { get; set; }
        public Packing Packing { get; set; }        
        public MarketPlaceEnum? MarketPlace { get; set; }
        public string? Observation { get; set; } 
    }
}
