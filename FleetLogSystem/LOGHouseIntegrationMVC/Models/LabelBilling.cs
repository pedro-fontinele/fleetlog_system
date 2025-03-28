using LOGHouseSystem.Infra.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class LabelBilling
    {
        public int Id { get; set; }

        public decimal Value { get; set; }

        public LabelBillingEnum Status { get; set; }

        public string Description { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }

        public Client? Client { get; set; }

        [ForeignKey("ReceiptNote")]
        public int? ReceiptNoteId { get; set; }

        public virtual ReceiptNote? ReceiptNote { get; set; }
    }
}
