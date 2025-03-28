using LOGHouseSystem.Infra.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.Models
{
    public class Packing
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Responsável")]
        public string Responsible { get; set; }

        [DisplayName("Descrição")]
        public string Description { get; set; }

        [DisplayName("Quantidade de Produtos")]
        public decimal? Quantity { get; set; }

        [DisplayName("Observação")]
        public string? Observation { get; set; }

        [DisplayName("Status")]
        public PackingStatus Status { get; set; }

        [DisplayName("Data de Criação")]
        public DateTime CreatedAt { get; set; }

        [DisplayName("Imagem")]
        public string? ImagePath { get; set; }

        [ForeignKey("Client")]
        public int? ClientId { get; set; }

        public virtual Client? Client { get; set; }

        [ForeignKey("ExpeditionOrder")]
        public int? ExpeditionOrderId { get; set; }

        public virtual ExpeditionOrder? ExpeditionOrder { get; set; }

        public List<PackingHistory> PackingHistories { get; set; }
        public PackingListTransportation PackingListTransportation { get; set; }

        public List<PackingItem>? Items { get; set; }


    }
}
