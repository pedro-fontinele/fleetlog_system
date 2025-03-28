using LOGHouseSystem.Models;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class ExpeditionOrderGroup
    {
        [DisplayName("CNPJ")]
        public string? Cnpj { get; set; }

        [DisplayName("Id do Cliente")]
        public int ClientId { get; set; }

        [DisplayName("Depositante")]
        public string? ClientName { get; set; }

        [DisplayName("Quantidade de Pedidos")]
        public int OrdersQuantity { get; set; }

        [DisplayName("Valor Total")]
        public decimal? TotalValue { get; set; }

        public List<int>? Orders { get; set; }

        public List<ExpeditionOrder>? ExpeditionOrders { get; set; }
    }
}
