using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Helpers;

namespace LOGHouseSystem.Models
{
    public class User
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
        [Required(ErrorMessage = "É necessário informar a senha do usuário")]
        public string Password { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "É necessário informar o email do usuário")]
        public string Email { get; set; }

        [DisplayName("Status")]
        [Required(ErrorMessage = "É necessário informar se o usuário está ativo")]
        public Status IsActive { get; set; }

        [DisplayName("Permissão")]
        [Required(ErrorMessage = "É necessário informar a permissão do usuário")]
        public PermissionLevel PermissionLevel { get; set; }

        public YesOrNo FirstAcess { get; set; }

        public bool ValidPassword(string password)
        {
            return Password == password.Hash();
        }

        public void SetPasswordHash()
        {
            Password = Password.Hash();
        }

        public void SetNewPasswordHash(string password)
        {
            Password = password.Hash();
        }

        public string GenerateNewPassword()
        {
            string newPassword = Guid.NewGuid().ToString().Substring(0, 8);
            Password = newPassword.Hash();
            return newPassword;
        }
    }
}
