using LOGHouseSystem.Infra.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.Models
{
    public class PackingItem
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Quantidade de Produtos")]
        public decimal? Quantity { get; set; }

        [DisplayName("Quantidade Validada")]
        public decimal? ValidatedQuantity { get; set; }

        [DisplayName("Endereço")]
        public string? Address { get; set; }

        [DisplayName("Status")]
        public PackingItemStatus Status { get; set; }

        [ForeignKey("Packing")]
        public int PackingId { get; set; }

        //public virtual Packing Packing { get; set; }

        [ForeignKey("Product")]
        public int? ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
