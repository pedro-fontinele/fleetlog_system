using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.Models
{
    public class ReturnInvoiceProductInvoices
    {
        [Key]
        public int Id { get; set; }
        public int ReturnInvoiceId { get; set; }
        public string XmlPath { get; set; }
        public string InvoiceAccessKey { get; set; }
    }
}
