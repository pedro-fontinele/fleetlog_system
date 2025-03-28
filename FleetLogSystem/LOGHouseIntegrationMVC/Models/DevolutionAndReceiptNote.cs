using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class DevolutionAndReceiptNote
    {
        public int Id { get; set; }

        [ForeignKey("ReceiptNote")]
        public int ReceiptNoteId { get; set; }
        public ReceiptNote ReceiptNote { get; set; }

        [ForeignKey("Devolution")]
        public int DevolutionId { get; set; }
        public Devolution Devolution { get; set; }
    }
}
