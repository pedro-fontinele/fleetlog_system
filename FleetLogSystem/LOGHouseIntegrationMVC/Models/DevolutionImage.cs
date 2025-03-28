using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class DevolutionImage
    {
        public int Id { get; set; }
        public string FilePath { get; set; }

        [ForeignKey("Devolution")]
        public int DevolutionId { get; set; } 
        public Devolution Devolution { get; set; }
    }
}
