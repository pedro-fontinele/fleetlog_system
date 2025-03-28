using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.ViewModels
{
    public class PickingListWithUrlAndArrayByteViewModel
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Responsável")]
        public string Responsible { get; set; }

        [DisplayName("Descrição")]
        public string Description { get; set; }

        [DisplayName("Quantidade")]
        public decimal? Quantity { get; set; }

        [DisplayName("Status")]
        public PickingListStatus Status { get; set; }

        [DisplayName("Data de Criação")]
        public DateTime CreatedAt { get; set; }

        [DisplayName("Prioridade")]
        public PriorityEnum? Priority { get; set; }

        public MarketPlaceEnum? MarketPlace { get; set; }

        public virtual List<PickingListItem> PickingListItems { get; set; }
        public virtual List<ExpeditionOrder> ExpeditionOrder { get; set; }

        [ForeignKey("Cart")]
        public int? CartId { get; set; }

        public Models.Cart? Cart { get; set; }

        public byte[]? Bytes { get; set; }

        public string? Url { get; set; }

        public bool? ModalActive { get; set; } = false;
    }
}
