using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class DevolutionAndProduct
    {
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [ForeignKey("Devolution")]
        public int DevolutionId { get; set; }

        public Devolution? Devolution { get; set; }
    }
}
