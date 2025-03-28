using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class BlingOrderItem
    {
        [Key]
        public virtual int Id { get; set; }

        [ForeignKey("BlingOrder")]
        public virtual int BlingOrderId { get; set; }
        public BlingOrder BlingOrder { get; set; }

        [DisplayName("Código")]
        public virtual string Code { get; set; }
        [DisplayName("Descrição")]
        public virtual string Description { get; set; }

        [DisplayName("Unidade")]
        public virtual string Unit { get; set; }

        [DisplayName("Quantidade")]
        public virtual decimal Quantity { get; set; }

        [DisplayName("Valor Unitario")]
        public virtual decimal Value { get; set; }

        [DisplayName("Código de Barras EAN / Gtin")]
        public virtual string? Gtin { get; set; }

        [DisplayName("Id do Produto")]
        [ForeignKey("Product")]
        public int? ProductId { get; set; }
    }
}
