using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class ReturnInvoiceOrders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("ReturnInvoice")]
        public int ReturnInvoiceId { get; set; }

        public ReturnInvoice ReturnInvoice { get; set; }


        [ForeignKey("ExpeditionOrder")]
        public int ExpeditionOrderId { get; set; }

        public ExpeditionOrder ExpeditionOrder { get; set; }


        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public decimal Quantity { get; set; }
        public DateTime EntryDate { get; set; }
    }
}
