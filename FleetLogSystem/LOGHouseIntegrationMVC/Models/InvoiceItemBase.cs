using LOGHouseSystem.Infra.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.Models
{
    public class InvoiceItemBase
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Código Produto (SKU)")]
        public string Code { get; set; }

        [DisplayName("Descrição")]
        public string? Description { get; set; }

        [DisplayName("Código EAN")]
        public string? Ean { get; set; }

        [DisplayName("Quantidade")]
        public double Quantity { get; set; }

        [DisplayName("Quantidade Recebida")]
        public double QuantityInspection { get; set; }

        [DisplayName("Valor Unitario")]
        public decimal? Value { get; set; }

        [DisplayName("Status Item")]
        public NoteItemStatus ItemStatus { get; set; }

        public string? PositionAddress { get; set; }
        public string? Validade { get; set; }
        public string? Lote { get; set; }
    }
}
