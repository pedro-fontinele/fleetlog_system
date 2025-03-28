using LOGHouseSystem.Infra.Enums;
using Microsoft.Build.Framework;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace LOGHouseSystem.Models
{
    public class Client
    {
        public int Id { get; set; }

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

        public YesOrNo IsActive { get; set; } = YesOrNo.Yes;

        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
