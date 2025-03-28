namespace LOGHouseSystem.ViewModels
{
    public class ReturnInvoiceOrdersResponseViewModel
    {
        public string InvoiceAccessKey { get; set; }
        public decimal Quantity { get; set; }
        public string Product { get; set; }
        public int ReturnInvoiceId { get; set; }
    }
}
