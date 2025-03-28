using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class FilterPackingViewModel
    {
        [DisplayName("Número da Nota")]
        public int? InvoiceNumber { get; set; }
        public int? ClientId { get; set; }

        [DisplayName("Cliente")]
        public string? ClientName { get; set; }

        [DisplayName("Status")]
        public PackingStatus? Status { get; set; }

        [DisplayName("Número da Lista de Separação")]
        public int? PickingListId { get; set; }

        [DisplayName("Data")]
        public DateTime? CreatedAt { get; set; }

        public int Page { get; set; } = 1;
    }
}
