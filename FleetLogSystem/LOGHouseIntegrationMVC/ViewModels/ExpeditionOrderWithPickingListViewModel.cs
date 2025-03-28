using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class ExpeditionOrderWithPickingListViewModel
    {
        public int Id { get; set; }

        [DisplayName("Depositante")]
        public string? ClientName { get; set; }


        [DisplayName("CNPJ")]
        public string? Cnpj { get; set; }

        [DisplayName("Id do Ecommerce")]
        public string? ExternalNumber { get; set; }

        [DisplayName("Origem do pedido")]
        public OrderOrigin? OrderOrigin { get; set; }

        [DisplayName("Chave de Acesso da Nota Fiscal")]
        public string? InvoiceAccessKey { get; set; }

        [DisplayName("Data da emissão")]
        public DateTime? IssueDate { get; set; }

        [DisplayName("Data da entrega")]
        public DateTime? DeliveryDate { get; set; }

        [DisplayName("Status")]
        public ExpeditionOrderStatus? Status { get; set; }

        [DisplayName("Transportadora")]
        public string? ShippingCompany { get; set; }

        [DisplayName("Observação do pedido")]
        public string? Obs { get; set; }

        [DisplayName("Metodo de Envio")]
        public ShippingMethodEnum? ShippingMethod { get; set; }

        [DisplayName("Código do pedido no metodo de envio")]
        public string? ShippingMethodCodeOrder { get; set; }

        [ForeignKey("Client")]
        public int? ClientId { get; set; }

        public virtual List<ExpeditionOrderItem>? ExpeditionOrderItems { get; set; }

        [DisplayName("NF")]
        public int InvoiceNumber { get; internal set; }

        [DisplayName("Status etiquetas")]
        public bool ShippingTagBlocked { get; set; }


        [ForeignKey("ShippingDetails")]
        public int? ShippingDetailsId { get; set; }

        public virtual ShippingDetails? ShippingDetails { get; set; }

        public ExpeditionOrderTagShipping? ExpeditionOrderTagShipping { get; set; }

        public virtual PickingList? PickingList { get; set; }
    }
}
