using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.Models
{
    public class AddressingStreet
    {
        public int AddressingStreetID { get; set; }

        [DisplayName("Rua")]
        [Required(ErrorMessage = "É necessário informar nome da Rua")]
        public string? Name { get; set; }

        public Status Status { get; set; }

        public virtual List<AddressingPosition> Positions { get; set; }
    }
}
