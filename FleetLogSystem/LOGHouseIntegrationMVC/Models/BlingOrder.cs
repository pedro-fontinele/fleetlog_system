using LOGHouseSystem.Infra.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class BlingOrder
    {
        [DisplayName("Id")]
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(Order = 1)]        

        [DisplayName("Código do Bling")]
        public string BlingNumber { get; set; }

        [DisplayName("Chave de Acesso da Nota Fiscal")]
        public string InvoiceAccessKey { get; set; }

        [DisplayName("Numero")]
        public string InvoiceNumber { get; set; }

        [DisplayName("Serie")]
        public string InvoiceSerie { get; set; }

        [DisplayName("Data da emissão")]
        public virtual DateTime? IssueDate { get; set; }

        [DisplayName("Valor da nota")]
        public virtual float? InvoiceValue { get; set; }

        [DisplayName("Observação do pedido")]
        public virtual string? Obs { get; set; }

        [DisplayName("Observação Interna")]
        public virtual string? ObsInternal { get; set; }

        [DisplayName("Data de Entrada")]
        public DateTime? EntryDate { get; set; }

        [ForeignKey("Client")]
        public int? ClientId { get; set; }
        public virtual List<BlingOrderItem> BlingOrderItems { get; set; }
    }
}
