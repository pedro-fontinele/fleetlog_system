namespace LOGHouseSystem.ViewModels
{
    public class SendEmailAboutOrderViewModel
    {
        public string subject { get; set; }
        public string message { get; set; }
        public int orderId { get; set; }
    }
}
