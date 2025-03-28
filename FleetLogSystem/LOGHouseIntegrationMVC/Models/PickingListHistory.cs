using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class PickingListHistory
    {
        public int Id { get; set; }

        [DisplayName("Observação")]
        public string? Observation { get; set; }

        [DisplayName("Data de Modificação")]
        public DateTime Date { get; set; }

        public PickingListStatus Status { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User? User { get; set; }

        [ForeignKey("PickingList")]
        public int PickingListId { get; set; }

        public PickingList? PickingList { get; set; }
    }
}
