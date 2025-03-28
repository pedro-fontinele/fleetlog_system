using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class PickingListItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("PickingList")]
        public int PickingListId { get; set; }

        //public virtual PickingList PickingList { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        [DisplayName("Quantidade")]
        public int Quantity { get; set; }

        [DisplayName("Quantidade Recebida")]
        public int QuantityInspection { get; set; }

        [DisplayName("Endereço")]
        public string Address { get; set; }

        [DisplayName("Status Item")]
        public PickingListItemStatus ItemStatus { get; set; }

    }
}
