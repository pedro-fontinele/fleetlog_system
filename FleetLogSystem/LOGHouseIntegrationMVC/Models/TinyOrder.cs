using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class TinyOrder
    {
        [Key]
        public virtual int Id { get; set; }

        [DisplayName("CNPJ")]
        public virtual string? Cnpj { get; set; }

        [DisplayName("Id do Ecommerce")]
        public virtual string? EcommerceId { get; set; }

        [DisplayName("Chave de Acesso da Nota Fiscal")]
        public virtual string? InvoiceAccessKey { get; set; }

        [DisplayName("Numero")]
        public virtual string? Number { get; set; }

        [DisplayName("Serie")]
        public virtual int? Serie { get; set; }

        [DisplayName("Url da Danfe")]
        public virtual string? UrlDanfe { get; set; }

        [DisplayName("Id do Pedido Ecommerce")]
        public virtual string? EcommerceOrderId { get; set; }

        [DisplayName("Data da emissão")]
        public virtual DateTime? IssueDate { get; set; }

        [DisplayName("Valor da nota")]
        public virtual float? InvoiceValue { get; set; }

        [DisplayName("Id da nota fiscal Tiny")]
        public virtual int? InvoiceTinyId { get; set; }

        [DisplayName("Observação do pedido")]
        public virtual string? Obs { get; set; }

        [DisplayName("Data de Entrada")]
        public DateTime? EntryDate { get; set; }

        [ForeignKey("Client")]
        public int? ClientId { get; set; }

        public virtual List<TinyOrderItem> TinyOrderItems { get; set; }
    }
}
