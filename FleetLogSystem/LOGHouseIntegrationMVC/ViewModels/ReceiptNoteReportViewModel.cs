using LOGHouseSystem.Models;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class ReceiptNoteReportViewModel
    {
        [DisplayName("Data de entrada inicial")]
        public int? ClientId { get; set; }
        public User UserLoged { get; set; }
        [DisplayName("Data de entrada inicial")]
        public DateTime? EntryDateStart { get; set; }
        [DisplayName("Data de entrada final")]
        public DateTime? EntryDateEnd { get; set; }
    }
}
