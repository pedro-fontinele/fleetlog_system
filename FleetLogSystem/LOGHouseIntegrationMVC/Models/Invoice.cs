using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class Invoice : InvoiceBase
    {

        [ForeignKey("ExpeditionOrder")]
        public int? ExpeditionOrderId { get; set; }

        public ExpeditionOrder? ExpeditionOrder { get; set; }

        public virtual List<InvoiceItem> InvoiceItems { get; set; }


    }
}
