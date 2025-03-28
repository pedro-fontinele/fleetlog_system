using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Helpers;

namespace LOGHouseSystem.Models
{
    public class ExpeditionOrder
    {
        [Key]
        public  int Id { get; set; }

        [DisplayName("Depositante")]
        public  string? ClientName { get; set; }

        [DisplayName("CNPJ")]
        public  string? Cnpj { get; set; }

        [DisplayName("Id do Ecommerce")]
        public string? ExternalNumber { get; set; }

        [DisplayName("Origem do pedido")]
        public OrderOrigin? OrderOrigin { get; set; }

        public ExpeditionOrderCreateOrigin CreateOrigin { get; set; }

        [DisplayName("Chave de Acesso da Nota Fiscal")]
        public string? InvoiceAccessKey { get; set; }

        [DisplayName("Data da emissão")]
        public DateTime? IssueDate { get; set; }

        [DisplayName("Data da entrega")]
        public DateTime? DeliveryDate { get; set; }

        [DisplayName("Data da Movimentação")]
        public DateTime? CreationDate { get; set; } = DateTimeHelper.GetCurrentDateTime();

        [DisplayName("Data da Finalização")]
        public DateTime? FinalizeDate { get; set; }

        [DisplayName("Status")]
        public ExpeditionOrderStatus? Status { get; set; }

        [DisplayName("Transportadora")]
        public string? ShippingCompany { get; set; }

        [DisplayName("Valor da Nota")]
        public decimal? InvoiceValue { get; set; }

        [DisplayName("Observação do pedido")]
        public string? Obs { get; set; }
        
        [DisplayName("Metodo de Envio")]
        public ShippingMethodEnum? ShippingMethod { get; set; }

        [DisplayName("Erros")]
        public string? Errors { get; set; }

        [DisplayName("Código do pedido no metodo de envio")]
        public string? ShippingMethodCodeOrder { get; set; }

        public bool ShippingTagBlocked { get; set; }

        [ForeignKey("Client")]
        public int? ClientId { get; set; }
        public virtual Client Client { get; set; }

        public virtual List<ExpeditionOrderItem>? ExpeditionOrderItems { get; set; }


        [ForeignKey("ShippingDetails")]
        public int? ShippingDetailsId { get; set; }

        public virtual ShippingDetails? ShippingDetails { get; set; }

        public ExpeditionOrderTagShipping? ExpeditionOrderTagShipping { get; set; }

        [DisplayName("Número da Nota")]
        public int InvoiceNumber { get; internal set; }

        public int InvoiceSerie { get; internal set; }

        [ForeignKey("PickingListId")]
        public int? PickingListId { get; set; }

        public int VolumeQuantity { get; set; }

        [ForeignKey("PackingListTransportationId")]
        public int? PackingListTransportationId { get; set; }

        public YesOrNo ReturnedInvoiceGenerated { get; set; } = YesOrNo.No;

        //[ForeignKey("ReturnInvoice")]
        //public int? ReturnInvoiceId { get; set; }

        //public PickingList? PickingList { get; set; }

        //public Packing? Packing { get; set; }
    }
}
