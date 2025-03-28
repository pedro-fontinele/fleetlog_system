using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.ViewModels
{
    public class ExpeditionOrderViewModel
    {
        [Key]
        public int? Id { get; set; }

        [DisplayName("Depositante")]
        [Required(ErrorMessage = "Informe o nome do depositante")]
        public string? ClientName { get; set; }

        [DisplayName("CNPJ")]
        [Required(ErrorMessage = "Informe o CNPJ")]
        public string? Cnpj { get; set; }

        [DisplayName("Id do Ecommerce")]
        public string? ExternalNumber { get; set; }

        [DisplayName("Origem do pedido")]
        public OrderOrigin? OrderOrigin { get; set; }

        [DisplayName("Chave de Acesso da Nota Fiscal")]
        public string? InvoiceAccessKey { get; set; }

        [DisplayName("Número da Nota")]
        public int InvoiceNumber { get;  set; }
        public int InvoiceSerie { get;  set; }

        [DisplayName("Data da emissão")]
        public DateTime? IssueDate { get; set; }

        [DisplayName("Data da entrega")]
        public DateTime? DeliveryDate { get; set; }

        [DisplayName("Valor da Nota")]
        public string? InvoiceValue { get; set; }

        [DisplayName("Status")]
        public ExpeditionOrderStatus Status { get; set; }

        [DisplayName("Transportadora")]
        [Required(ErrorMessage = "Informe o nome da Transportadora")]
        public string? ShippingCompany { get; set; }

        [DisplayName("Observação do pedido")]
        public string? Obs { get; set; }

        [DisplayName("Metodo de Envio")]
        public ShippingMethodEnum? ShippingMethod { get; set; }

        [ForeignKey("Client")]
        public int? ClientId { get; set; }

        public virtual List<ExpeditionOrderItem>? ExpeditionOrderItems { get; set; }

        //ShippingDetails

        [DisplayName("Nome")]
        [Required(ErrorMessage = "Informe o nome")]
        public string? Name { get; set; }

        [DisplayName("Nome Fantasia")]
        public string? FantasyName { get; set; }

        [DisplayName("CPF/CNPJ")]
        [Required(ErrorMessage = "Informe o CPF/CNPJ")]
        public string? CpfCnpj { get; set; }

        [DisplayName("RG")]
        [Required(ErrorMessage = "Informe o RG")]
        public string? Rg { get; set; }

        [DisplayName("Endereço de Entrega")]
        [Required(ErrorMessage = "Informe o endereço de entrega")]
        public string? Address { get; set; }

        [DisplayName("Número")]
        [Required(ErrorMessage = "Informe o número do local")]
        public string? Number { get; set; }

        [DisplayName("Complemento")]
        public string? Complement { get; set; }

        [DisplayName("Bairro")]
        [Required(ErrorMessage = "Informe o bairro")]
        public string? Neighborhood { get; set; }

        [DisplayName("CEP")]
        [Required(ErrorMessage = "Informe o CEP")]
        public string? Cep { get; set; }

        [DisplayName("Cidade")]
        [Required(ErrorMessage = "Informe a cidade")]
        public string? City { get; set; }

        [DisplayName("UF")]
        [Required(ErrorMessage = "Informe a UF")]
        public string? Uf { get; set; }

        [DisplayName("Telefone")]
        [Required(ErrorMessage = "Informe o número de telefone")]
        public string? Phone { get; set; }
        public int? ShippingDetailsId { get; set; }
        public IFormFile File { get; set; }

        //UserLoged

        public User? UserLoged { get; set; }
    }
}
