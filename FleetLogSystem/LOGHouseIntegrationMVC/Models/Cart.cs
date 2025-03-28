using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.Models
{
    public class Cart
    {
        public int Id { get; set; }

        [DisplayName("Descrição")]
        [Required(ErrorMessage ="É necessário informar uma descrição para o carrinho!")]
        public string Description { get; set; }

        [DisplayName("Data de Criação")]
        public DateTime CreatedAt { get; set; }
    }
}
