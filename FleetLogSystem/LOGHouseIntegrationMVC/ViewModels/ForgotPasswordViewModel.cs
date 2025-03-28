using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.ViewModels
{
    public class ForgotPasswordViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Digite o email do usuário")]
        [EmailAddress(ErrorMessage ="Digite um email válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Confirme o email do usuário")]
        [Compare("Email", ErrorMessage = "Os emails não conferem")]
        [EmailAddress(ErrorMessage = "Digite um email válido")]
        public string ConfirmEmail { get; set; }
    }
}
