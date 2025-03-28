using LOGHouseSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace LOGHouseSystem.ViewModels
{
    public class SentEmailViewModel
    {
        public int Id { get; set; }

        [DisplayName("Título")]
        public string Title { get; set; }

        [DisplayName("Corpo")]
        public string Body { get; set; }

        [DisplayName("Destinatário")]
        public string To { get; set; }

        [DisplayName("Email do Destinatário")]
        public string ToEmail { get; set; }

        [DisplayName("Numero da Nota Fiscal")]
        public int? InvoiceNumber { get; set; }

        [DisplayName("Data de Envio")]
        public DateTime SendData { get; set; }

        [ForeignKey("Client")]
        public int? ClientId { get; set; }

        public Client? Client { get; set; }

    }
}
