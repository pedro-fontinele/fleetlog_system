using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class FilterViewModel
    {
        [DisplayName("CNPJ")]
        public string? Cnpj { get ; set; }

        [DisplayName("Depositante")]
        public string? SocialReason { get ; set; }

        [DisplayName("Numero NF")]
        public string InvoiceNumber { get; set; }

        [DisplayName("Status")]
        public NoteStatus? NoteStatus { get; set; }


        [DisplayName("ClientId")]
        public int? ClientId { get; set; }

        [DisplayName("Data de Entrada Inicial")]
        public DateTime? EntryDateStart { get; set; }

        [DisplayName("Data de Entrada Final")]
        public DateTime? EntryDateEnd { get; set; }

        [DisplayName("Data de Emissão")]
        public DateTime? IssueDate { get; set; }

        public int Page { get; set; } = 1;
    }
}
