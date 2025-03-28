using LOGHouseSystem.Adapters.Extensions.BlingExtension.Dto.Hook.Request;
using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class OrderIntegrationViewModel
    {
        [DisplayName("Data de entrada inicial")]
        public int? ClientId { get; set; }
        [DisplayName("Data de entrada inicial")]
        public DateTime? EntryDateStart { get; set; }
        [DisplayName("Data de entrada final")]
        public DateTime? EntryDateEnd { get; set; }
        [DisplayName("Origem do pedido")]
        public OrderOrigin? OrderOrigin { get; set; }

        public List<BlingNotaCallbackRequest> Orders { get; set; } = new List<BlingNotaCallbackRequest>();
    }
}
