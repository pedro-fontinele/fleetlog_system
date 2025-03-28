using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class ExpeditionOrderTagShipping
    {
        [Key]
        [DisplayName("Código")]
        public int Id { get; set; }

        [DisplayName("Código da Entrega")]
        public string ShippingCode { get; internal set; }

        [DisplayName("Chave de Acesso da Nota Fiscal")]
        public string? InvoiceAccessKey { get; set; }

        [DisplayName("Origem da etiqueta")]
        public ShippingMethodEnum? OrderTagOrigin { get; set; }

        [DisplayName("Tipo da etiqueta")]
        public FileFormatEnum FileFormat { get; set; }

        [DisplayName("Url da etiqueta")]
        public string Url { get; set; }

        [DisplayName("Id do Pedido")]
        [ForeignKey("ExpeditionOrder")]
        public int? ExpeditionOrderId { get; set; }

        [DisplayName("Data de Inserção")]
        public DateTime EntryDate { get; internal set; }
    }
}
