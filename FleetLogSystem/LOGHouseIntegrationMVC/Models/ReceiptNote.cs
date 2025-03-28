using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class ReceiptNote : InvoiceBase
    {
        public YesOrNo IsDevolution { get; set; } = 0;

        [ForeignKey("Client")]
        public int ClientId { get; set; }

        public virtual Client Client { get; set; }        

        public virtual List<ReceiptNoteItem> ReceiptNoteItems { get; set; }

    }
}
