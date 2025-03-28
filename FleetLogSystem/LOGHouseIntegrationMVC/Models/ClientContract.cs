using DocumentFormat.OpenXml.Drawing.Charts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace LOGHouseSystem.Models
{
    public class ClientContract
    {
        public int Id { get; set; }

        [DisplayName("Quantidade de Pallets Contratados")]
        [Required(ErrorMessage = "É necessário informar a Quantidade de Pallets Contratados")]
        public int? Storage { get; set; }

        [DisplayName("Valor do Pallet Excedente")]
        [Required(ErrorMessage = "É necessário informar o Valor do Pallet Excedente")]
        public decimal? SurplusStorage { get; set; }

        [DisplayName("Valor Total de Armazenagem")]
        [Required(ErrorMessage = "É necessário informar o Valor Total da Armazenagem")]
        public decimal? StorageValue { get; set; }

        [DisplayName("Quantidade de Pedidos Por Mês")]
        [Required(ErrorMessage = "É necessário informar os Pedidos")]
        public int? Requests { get; set; }

        [DisplayName("Valor Total de Pedidos")]
        [Required(ErrorMessage = "É necessário informar o Valor Total dos Pedidos")]
        public decimal? RequestsValue { get; set; }

        [DisplayName("Porcentagem de Seguro")]
        public decimal? InsurancePercentage { get; set; }

        [DisplayName("Valor do Pedido Excedente")]
        public decimal? ExcessOrderValue { get; set; }

        [DisplayName("Quantidade Máxima de Unidades Por Envio")]
        [Required(ErrorMessage = "É necessário informar Qtd. Máxima de Unidades por Envio")]
        public int? ShippingUnits { get; set; }

        [DisplayName("Valor Total do Contrato")]
        [Required(ErrorMessage = "É necessário informar o Valor Total do Contrato")]
        public decimal? ContractValue { get; set; }

        public int ClientId { get; set; }

        public virtual Client Client { get; set; }
    }
}
