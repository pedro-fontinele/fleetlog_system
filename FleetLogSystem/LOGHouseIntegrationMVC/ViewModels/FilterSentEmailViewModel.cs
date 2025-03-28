using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class FilterSentEmailViewModel
    {

        [DisplayName("Cliente")]
        public string ClientName { get; set; }
        [DisplayName("Título")]
        public string Title { get; set; }

        [DisplayName("Email do Destinatário")]
        public string ToEmail { get; set; }

        [DisplayName("Data de Envio")]
        public DateTime? SendData { get; set; }

        [DisplayName("Cliente")]
        public int? ClientId { get; set; }
        public int Page { get; set; } = 1;

    }
}
