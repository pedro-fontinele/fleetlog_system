using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.Models
{
    public class ExpeditionOrderItem
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Nome")]
        public string? Name { get; set; }

        [DisplayName("Quantidade")]
        public decimal Quantity { get; set; }

        [DisplayName("Descrição")]
        public string? Description { get; set; }

        [DisplayName("Id do Ecommerce")]
        public string? ExternalNumberItem { get; set; }

        [DisplayName("Valor Unitario")]
        public decimal? Value { get; set; }

        [DisplayName("Código de Barras EAN / Gtin")]
        public string? Ean { get; set; }

        [DisplayName("Id do Produto")]
        [ForeignKey("Product")]
        public int? ProductId { get; set; }

        [DisplayName("Id do Pedido")]
        [ForeignKey("ExpeditionOrder")]
        public int? ExpeditionOrderId { get; set; }

        public ExpeditionOrder ExpeditionOrder { get; set; }
    }
}
