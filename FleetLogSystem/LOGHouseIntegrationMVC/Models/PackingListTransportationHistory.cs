using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class PackingListTransportationHistory
    {
        public int Id { get; set; }

        [DisplayName("Data")]
        public DateTime Date { get; set; }

        public PackingListTransportationStatus Status { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User? User { get; set; }

        public int? PackingListTransportationId { get; set; }
        public PackingListTransportation PackingListTransportation { get; set; }
    }
}
