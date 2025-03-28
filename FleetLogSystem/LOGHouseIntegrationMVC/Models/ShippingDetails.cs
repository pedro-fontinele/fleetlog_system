using LOGHouseSystem.Infra.Enums;
using MessagePack;
using Microsoft.Build.Framework;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class ShippingDetails
    {
        
        public int Id { get; set; }

        [DisplayName("Nome")]
        public string? Name { get; set; }

        [DisplayName("Nome Fantasia")]
        public string? FantasyName { get; set; }

        [DisplayName("CPF/CNPJ")]
        public string? CpfCnpj { get; set; }

        [DisplayName("RG")]
        public string? Rg { get; set; }

        [DisplayName("Endereço de Entrega")]
        public string? Address { get; set; }

        [DisplayName("Número")]
        public string? Number { get; set; }

        [DisplayName("Complemento")]
        public string? Complement { get; set; }

        [DisplayName("Bairro")]
        public string? Neighborhood { get; set; }

        [DisplayName("CEP")]
        public string? Cep { get; set; }

        [DisplayName("Cidade")]
        public string? City { get; set; }

        [DisplayName("UF")]
        public string? Uf { get; set; }

        [DisplayName("Telefone")]
        public string? Phone { get; set; }
    }
}
