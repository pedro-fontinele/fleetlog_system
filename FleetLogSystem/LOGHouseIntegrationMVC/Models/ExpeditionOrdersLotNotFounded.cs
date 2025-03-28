using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class ExpeditionOrdersLotNotFounded
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("ExpeditionOrder")]
        public int ExpeditionOrderId { get; set; }        
        public ExpeditionOrder ExpeditionOrder { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        [DisplayName("Quantidade pendente")]
        public decimal Quantity { get; set; }
        public ExpeditionOrdersLotNotFoundedStatusEnum Status { get; set; }

        [DisplayName("Data de criação")]
        public DateTime EntryDate { get; set; }
    }
}
