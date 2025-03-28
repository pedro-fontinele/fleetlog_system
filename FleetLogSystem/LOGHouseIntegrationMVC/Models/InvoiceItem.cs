using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class InvoiceItem : InvoiceItemBase
    {
        [ForeignKey("Invoice")]
        public int InvoiceId { get; set; }
    }
}
