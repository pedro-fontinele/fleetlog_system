using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class ReturnInvoiceOrdersDetails
    {        
        public decimal? Quantity { get; set; }
        public string? Description { get; set; }
        public string? Ean { get; set; }
        public int InvoiceNumber { get; set; }
        public int ExpeditionId { get; set; }
        public int ProductId { get; set; }
    }
}
