using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class ExpeditionOrderItemViewModel
    {
        public int Id { get; set; }

        [DisplayName("Nome")]
        public string Name { get; set; }

        [DisplayName("Quantidade")]
        public decimal Quantity { get; set; }

        [DisplayName("Descrição")]
        public string? Description { get; set; }

        [DisplayName("Código de Barras EAN / Gtin")]
        public string? Ean { get; set; }

        [DisplayName("Id do Produto")]
        public int? ProductId { get; set; }

        [DisplayName("Id do Pedido")]
        public int ExpeditionOrderId { get; set; }

        public int ClientId { get; set; }
    }
}
