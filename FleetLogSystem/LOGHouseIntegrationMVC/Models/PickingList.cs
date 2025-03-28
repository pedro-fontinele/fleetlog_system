using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LOGHouseSystem.Models
{
    public class PickingList
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

        public MarketPlaceEnum? MarketPlace { get; set; }

        [DisplayName("Prioridade")]
        public PriorityEnum? Priority { get; set; }

        public virtual List<PickingListItem> PickingListItems { get; set; }

        [JsonIgnore]
        public virtual List<ExpeditionOrder> ExpeditionOrder { get; set; }
        public virtual List<PickingListHistory> PickingListHistories { get; set; }

        [ForeignKey("Cart")]
        public int? CartId { get; set; }

        public Cart? Cart { get; set; }
    }
}
