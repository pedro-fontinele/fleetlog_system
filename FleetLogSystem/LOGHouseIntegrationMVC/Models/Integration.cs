using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace LOGHouseSystem.Models
{
    public class Integration
    {
        public int Id { get; set; }

        [DisplayName("Nome")]
        [Required(ErrorMessage = "É necessário informar o nome da integração")]
        public string Name { get; set; }

        [DisplayName("Tipo")]
        [Required(ErrorMessage = "É necessário selecionar o tipo")]
        public IntegrationType Type { get; set; }

        public Status Status { get; set; }

        public int? ClientId { get; set; }

        public virtual Client? Client { get; set; }

        public Integration()
        {
            Status = Status.Ativo;
        }
    }
}
