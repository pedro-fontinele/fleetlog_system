using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class ReceiptNoteItem : InvoiceItemBase
    {
        public int? ProductId { get; set; }


        [ForeignKey("ReceiptNote")]
        public int ReceiptNoteId { get; set; }        

        public ReceiptNote ReceiptNote { get; set; }

        public YesOrNo LotGenerated { get; set; } = YesOrNo.No;

    }
}
