using LOGHouseSystem.Models;

namespace LOGHouseSystem.ViewModels
{
    public class ClientOrdersViewModel
    {
        public int ClientId { get; set; }
        public List<ExpeditionOrder> Orders { get; set; }
    }
}
