using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;


namespace LOGHouseSystem.Models
{
    public class InventoryMovement
    {
        public int Id { get; set; }

        [DisplayName("Data")]
        public DateTime Date { get; set; }

        [DisplayName("Tipo")]
        [Required(ErrorMessage = "É necessário informar o tipo")]
        public InventoryType Type { get; set; }

        [DisplayName("Quantidade")]
        [Required(ErrorMessage = "É necessário informar a Quantidade")]
        public double Quantity { get; set; }

        [DisplayName("Quantidade Final do Estoque")]        
        public double QuantityFinalQuantity { get; set; }

        [DisplayName("Observação")]
        public string? Note { get; set; }

        [DisplayName("Data de Criação")]
        public DateTime CreateAt { get; set; } = DateTime.Now;

        public int? ProductId { get; set; }

        public Product? Product { get; set; }
        public bool Status { get; set; }

        public StockSlotMovimentEnum StockSlotMoviment { get; set; }
        
        public OriginInventoryMovimentEnum Origin { get; set; }
    }
}
