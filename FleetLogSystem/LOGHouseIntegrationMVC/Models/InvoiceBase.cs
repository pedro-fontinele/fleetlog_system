using LOGHouseSystem.Infra.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.Models
{
    public class InvoiceBase
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Número Nota")]
        public string Number { get; set; }

        [DisplayName("Série Nota")]
        public string SerialNumber { get; set; }

        [DisplayName("Chave Acesso NFe")]
        public string AccessKey { get; set; }

        [DisplayName("Documento Emitente")]
        public string? EmitDocument { get; set; }

        [DisplayName("Documento Destinatário")]
        public string? DestDocument { get; set; }

        [DisplayName("Data de Entrada")]
        public DateTime? EntryDate { get; set; }

        [DisplayName("Data de Emissão")]
        public DateTime? IssueDate { get; set; }

        [DisplayName("Status")]
        public NoteStatus Status { get; set; }

        public decimal? TotalInvoiceValue { get; set; }

    }
}
