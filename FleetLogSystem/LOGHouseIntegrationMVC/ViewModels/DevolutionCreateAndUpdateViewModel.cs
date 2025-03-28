
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class DevolutionCreateAndUpdateViewModel
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

        [DisplayName("Data de Entrada")]
        public DateTime EntryDate { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }

        [DisplayName("Nome do Cliente")]
        public string ClientName { get; set; }

        public List<string>? Products { get; set; }

        public bool IsCreation { get; set; } = true;
    }
}
