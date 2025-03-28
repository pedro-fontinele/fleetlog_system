using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class ProductWithStockAdress
    {
        public int Id { get; set; }

        [DisplayName("Código do Produto")]
        //[Required(ErrorMessage = "É necessário informar o código do produto")]
        public string Code { get; set; }

        [DisplayName("Descrição")]
        public string? Description { get; set; }

        [DisplayName("Código EAN")]
        public string? Ean { get; set; }

        [DisplayName("Quantidade em Estoque")]
        //[Required(ErrorMessage = "É necessário informar a quantidade do produto em estoque")]
        public double StockQuantity { get; set; }

        [DisplayName("Quantidade Reservada")]
        //[Required(ErrorMessage = "É necessário informar a quantidade do produto em estoque")]
        public double StockReservationQuantity { get; set; }

        [DisplayName("Estoque Total")]
        //[Required(ErrorMessage = "É necessário informar a quantidade do produto em estoque")]
        public double TotalStock { get; set; }


        [DisplayName("Posição")]
        public string? PositionName { get; set; }

        public int ClientId { get; set; }


    }
}
