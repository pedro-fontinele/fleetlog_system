using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class TinyOrderItem
    {
        [Key]
        public virtual int Id { get; set; }

        [ForeignKey("TinyOrder")]
        public virtual int TinyOrderId { get; set; }
        public TinyOrder TinyOrder { get; set; }

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
