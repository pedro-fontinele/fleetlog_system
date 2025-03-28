using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class FilterDevolutionViewModel
    {

        [DisplayName("Cliente")]
        public int? ClientName { get; set; }

        [DisplayName("Cliente")]
        public int? ClientId { get; set; }

        [DisplayName("Data")]
        public DateTime? Date { get; set; }

        [DisplayName("Status")]
        public DevolutionStatus? Status { get; set; }
        public int Page { get; set; } = 1;
    }
}
