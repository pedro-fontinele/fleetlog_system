using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class ExpeditionOrderFilterViewModel : PaginationRequest
    {
        [DisplayName("Depositante")]
        public string? ClientName { get; set; }

        [DisplayName("Cliente Id")]
        public int? ClientId { get; set; }


        [DisplayName("Id do Ecommerce")]
        public string? ExternalNumber { get; set; }

        [DisplayName("Origem do pedido")]
        public OrderOrigin? OrderOrigin { get; set; }

        [DisplayName("Data da entrada inicio")]
        public DateTime? IssueDateStart { get; set; }

        [DisplayName("Data da entrada final")]
        public DateTime? IssueDateEnd { get; set; }

        [DisplayName("Digite a data finalização inicial")]
        public DateTime? FinalizeStartDate { get; set; }

        [DisplayName("Digite a data finalização final")]
        public DateTime? FinalizeEndDate { get; set; }

        [DisplayName("Digite a data de movimentação inicial")]
        public DateTime? CreationStartDate { get; set; }
        [DisplayName("Digite a data de movimentação final")]
        public DateTime? CreationEndDate { get; set; }

        [DisplayName("Status")]
        public ExpeditionOrderStatus? Status { get; set; }

        [DisplayName("Transportadora")]
        public string? ShippingCompany { get; set; }

        [DisplayName("Metodo de Envio")]
        public ShippingMethodEnum? ShippingMethod { get; set; }

        [DisplayName("Status etiqueta")]
        public int? ShippingTagBlocked { get; set; }

        public string InvoiceNumber { get; set; }
        public ExpeditionOrdersPage PageFilter { get; set; }

        public Client Client { get; set; }

    }
}
