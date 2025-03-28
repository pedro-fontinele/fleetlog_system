using LOGHouseSystem.Infra.Enums;
using Newtonsoft.Json;
using System.ComponentModel;

namespace LOGHouseSystem.Models
{
    public class SmartgoImportation
    {
        public int Id { get; set; }

        public int IdDepositante { get; set; }

        public string SKU { get; set; }

        public string ProductName { get; set; }

        public string EAN { get; set; }

        public int Quantity { get; set; }

        public string PositionAddress { get; set; }

        public string? Lote { get; set; }
        public string? Validade { get; set; }

        public string UnitPrice { get; set; }

    }
}
