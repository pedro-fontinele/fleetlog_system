using LOGHouseSystem.Infra.Enums;

namespace LOGHouseSystem.ViewModels
{
    public class ReceiptNoteDashboardViewModel
    {
        public int Id { get; set; }
        public string Cnpj { get; set; }
        public string SocialName { get; set; }
        public string InvoiceNumber { get; set; }
        public NoteStatus Status { get; set; }
        public int ConcludedPercent { get; set; }
        public DateTime Date { get; set; }
        public string AccessKey { get; set; }
        public YesOrNo IsDevolution { get; set; }
    }
}
