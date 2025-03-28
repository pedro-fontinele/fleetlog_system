using LOGHouseSystem.Infra.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace LOGHouseSystem.Models
{
    public class ExpeditionOrderHistory
    {
        public int Id { get; set; }

        [DisplayName("Observação")]
        public string? Observation { get; set; }

        [DisplayName("Data de Modificação")]
        public DateTime Date { get; set; }

        public ExpeditionOrderStatus Status { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User? User { get; set; }

        [ForeignKey("ExpeditionOrder")]
        public int? ExpeditionOrderId { get; set; }

        public ExpeditionOrder? ExpeditionOrder { get; set; }
    }
}
