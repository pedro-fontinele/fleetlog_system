using LOGHouseSystem.Infra.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace LOGHouseSystem.Models
{
    public class PackingHistory
    {
        public int Id { get; set; }

        [DisplayName("Observação")]
        public string? Observation { get; set; }

        [DisplayName("Data de Modificação")]
        public DateTime Date { get; set; }

        public PackingStatus Status { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User? User { get; set; }

        [ForeignKey("Packing")]
        public int PackingId { get; set; }

        public Packing? Packing { get; set; }
    }
}
