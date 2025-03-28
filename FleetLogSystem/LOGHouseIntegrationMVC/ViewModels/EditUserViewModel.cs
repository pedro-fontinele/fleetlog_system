using LOGHouseSystem.Infra.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class EditUserViewModel
    {
        public int Id { get; set; }

        [DisplayName("Nome")]
        [Required(ErrorMessage = "É necessário informar o nome")]
        public string Name { get; set; }

        [DisplayName("Usuário")]
        [Required(ErrorMessage = "É necessário informar o nome de usuário")]
        public string Username { get; set; }

        [DisplayName("Senha")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }


        [DisplayName("Email")]
        [Required(ErrorMessage = "É necessário informar o email do usuário")]
        public string Email { get; set; }

        [DisplayName("Ativo")]
        [Required(ErrorMessage = "É necessário informar se o usuário está ativo")]
        public Status IsActive { get; set; }

        [DisplayName("Permissão")]
        [Required(ErrorMessage = "É necessário informar a permissão do usuário")]
        public PermissionLevel PermissionLevel { get; set; }

        public YesOrNo FirstAcess { get; set; }
    }
}
