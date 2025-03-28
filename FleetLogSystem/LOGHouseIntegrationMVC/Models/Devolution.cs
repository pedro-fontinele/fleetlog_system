using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class Devolution
    {
        public int Id { get; set; }

        [DisplayName("Nome do Remetente")]
        public string SenderName { get; set; }

        [DisplayName("Número da Nota")]
        public string InvoiceNumber { get; set; }

        [DisplayName("Número de Postagem")]
        public string PostNumber { get; set; }

        [DisplayName("Observação")]
        public string? Observation { get; set; }

        [DisplayName("Imagens")]
        public List<DevolutionImage>? Images { get; set; }

        public DevolutionStatus Status { get; set; }

        public DateTime EntryDate { get; set; } = DateTime.Now;

        [ForeignKey("Client")]
        public int ClientId { get; set; }

        [DisplayName("Cliente")]
        public Client? Client { get; set; }

        public List<DevolutionAndProduct>? Products { get; set; }

    }
}
