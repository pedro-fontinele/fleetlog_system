using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class ReturnInvoice
    {
        public int Id { get; set; }

        [DisplayName("Data de Criação")]
        public DateTime CreatedAt { get; set; }

        [DisplayName("Data de Emissão")]
        public DateTime? IssueDate { get; set; }

        [DisplayName("Status")]
        public ReturnInvoiceStatus Status { get; set; }

        [DisplayName("Id Externo")]
        public string? ExternalId { get; set; }

        [DisplayName("Valor")]
        public decimal? Value { get; set; }

        [DisplayName("Chave de Acesso da Nota")]
        public string? InvoiceAccessKey { get; set; }

        [DisplayName("Numero da Nota")]
        public int? InvoiceNumber { get; set; }

        [DisplayName("Rejeição")]
        public string? Rejection { get; set; }


        [ForeignKey("Client")]
        public int? ClientId { get; set; }
        public Client? Client { get; set; }

        public virtual List<ReturnInvoiceItem> ReturnInvoiceItems { get; set; }
        public string Xml { get; internal set; }
        public string LinkDanfe { get; internal set; }
        public string LinkPdf { get; internal set; }
    }
}
