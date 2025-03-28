using LOGHouseSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class ClientAndYourContractViewModel
    {
        public int ClientId { get; set; }

        [DisplayName("CNPJ")]
        [Required(ErrorMessage = "É necessário informar o CNPJ do Cliente")]
        public string Cnpj { get; set; }

        [DisplayName("E-mail")]
        [EmailAddress(ErrorMessage = "Digite um e-mail válido!")]
        [Required(ErrorMessage = "É necessário informar o E-mail do Cliente")]
        public string Email { get; set; }

        [DisplayName("Endereço do Cliente")]
        [Required(ErrorMessage = "É necessário informar o endereço do Cliente")]
        public string Adress { get; set; }

        [DisplayName("Razão Social")]
        [Required(ErrorMessage = "É necessário informar a Razão Soicial")]
        public string SocialReason { get; set; }

        [DisplayName("Telefone do Cliente")]
        [Required(ErrorMessage = "É necessário informar o número de telefone do Cliente")]
        [Phone(ErrorMessage = "Digite um número de telefone válido")]
        public string Phone { get; set; }

        [DisplayName("Inscrição Estadual")]
        public string? StateRegistration { get; set; }

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

        [DisplayName("Quantidade Máxima de Unidades Por Envio")]
        [Required(ErrorMessage = "É necessário informar Qtd. Máxima de Unidades por Envio")]
        public int? ShippingUnits { get; set; }

        [DisplayName("Valor Total do Contrato")]
        [Required(ErrorMessage = "É necessário informar o Valor Total do Contrato")]
        public decimal? ContractValue { get; set; }

        [DisplayName("Porcentagem de Seguro")]
        public decimal? InsurancePercentage { get; set; }

        [DisplayName("Valor do Pedido Excedente")]
        public decimal? ExcessOrderValue { get; set; }

        [DisplayName("Login")]
        public string? Login { get; set; }

        [DisplayName("Senha")]
        public string? Password { get; set; }

    }
}
