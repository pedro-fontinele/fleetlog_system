using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class ExpeditionOrderReportViewModel
    {
        [DisplayName("Digite o nome do cliente")]
        public int? ClientId { get; set; }
        [DisplayName("Digite a data de finalização inicial")]
        public DateTime? FinalizeStartDate { get; set; }
        [DisplayName("Digite a data finalização final")]
        public DateTime? FinalizeEndDate { get; set; }

        [DisplayName("Digite a data de movimentação inicial")]
        public DateTime? CreationStartDate { get; set; }
        [DisplayName("Digite a data de movimentação final")]
        public DateTime? CreationEndDate { get; set; }

        [DisplayName("Status do pedido")]
        public ExpeditionOrderStatus? Status { get; set; }

        public User UserLoged { get; set; }
    }
}
