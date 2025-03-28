using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.Models
{
    public class AddressingPosition
    {
        public int AddressingPositionID { get; set; }

        public int AddressingStreetID { get; set; }
        public virtual AddressingStreet AddressingStreet { get; set; }

        [DisplayName("Posição")]
        [Required(ErrorMessage = "É necessário informar o nome da posição")]
        public string? Name { get; set; }

        public AddressingPositionStatus Status { get; set; }
    }
}
