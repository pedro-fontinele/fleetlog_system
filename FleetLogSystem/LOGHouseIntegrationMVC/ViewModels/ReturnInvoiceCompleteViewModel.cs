using LOGHouseSystem.Models;

namespace LOGHouseSystem.ViewModels
{
    public class ReturnInvoiceCompleteViewModel
    {
        public ReturnInvoice Invoice { get; set; }
        public List<ReturnInvoiceOrdersDetails> Orders { get; set; }        
        public List<ReturnInvoiceProductInvoices> ReturnInvoiceProductInvoices { get; set; }        
    }
}
