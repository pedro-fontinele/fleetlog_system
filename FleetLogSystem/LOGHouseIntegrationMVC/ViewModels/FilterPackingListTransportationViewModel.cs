using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class FilterPackingListTransportationViewModel
    {
        [DisplayName("Número da Nota")]
        public int? InvoiceNumber { get; set; }
        public int? ShippingCompanyId { get; set; }

        [DisplayName("Transportadora")]
        public string? ShippingCompanyName { get; set; }

        [DisplayName("Status")]
        public PackingListTransportationStatus? Status { get; set; }

        [DisplayName("Data")]
        public DateTime? CreatedAt { get; set; }

        public int Page { get; set; } = 1;
    }
}
