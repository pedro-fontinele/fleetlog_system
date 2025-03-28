using LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Dto;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.ViewModels
{
    public class StockImportViewModel
    {
        public List<SmartgoImportation> StockItems { get; set; } = new List<SmartgoImportation>();

        public int ClientId { get; set; } = 0;

        public string ClientName { get; set; } = "";

        public int DepositanteId { get; set; } = 0;
    }
}
