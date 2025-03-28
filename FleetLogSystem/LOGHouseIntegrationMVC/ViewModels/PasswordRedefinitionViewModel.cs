using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.ViewModels
{
    public class PasswordRedefinitionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Digite a nova senha do usuário")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirme a senha do usuário")]
        [Compare("Password", ErrorMessage = "As duas senhas não são iguais")]
        public string ConfirmPassword { get; set; }
    }
}
