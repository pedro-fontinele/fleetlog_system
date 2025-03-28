using LOGHouseSystem.Infra.Helpers;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class FilterPackingWhithoutPackingListTrasportationViewModel
    {
        [DisplayName("Data Inicial")]
        public DateTime? CreatedAtStart { get; set; }

        [DisplayName("Data Final")]
        public DateTime? CreatedAtEnd { get; set; }

        public int Page { get; set; } = 1;
    }
}
