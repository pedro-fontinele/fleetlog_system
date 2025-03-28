using Microsoft.Build.Framework;
using System.ComponentModel;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;


namespace LOGHouseSystem.Models
{
    public class Product
    {
        public int Id { get; set; }

        [DisplayName("Código do Produto")]
        //[Required(ErrorMessage = "É necessário informar o código do produto")]
        public string Code { get; set; }

        [DisplayName("Descrição")]
        [Required(ErrorMessage = "É necessário informar a descrição do produto")]
        public string? Description { get; set; }

        [DisplayName("Código EAN")]
        [Required(ErrorMessage = "É necessário informar o EAN do produto")]
        public string? Ean { get; set; }

        [DisplayName("Quantidade em Estoque")]
        //[Required(ErrorMessage = "É necessário informar a quantidade do produto em estoque")]
        public double StockQuantity { get; set; }
        [DisplayName("Quantidade Reservada")]
        public double StockReservationQuantity { get; set; }

        [DisplayName("Data de Criação")]
        public DateTime CreatedAt { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; }

    }
}
