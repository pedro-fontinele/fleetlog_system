using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace LOGHouseSystem.Models
{
    public class ShippingCompany
    {
        public int Id { get; set; }

        [DisplayName("Nome")]
        [Required(ErrorMessage = "É necessário informar o nome da Transportadora")]
        public string Name { get; set; }

        [DisplayName("Status")]
        [Required(ErrorMessage = "É necessário informar o status da Transportadora")]
        public Status Active { get; set; }
    }
}
