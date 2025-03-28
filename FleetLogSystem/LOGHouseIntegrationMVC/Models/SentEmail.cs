using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class SentEmail
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

        [DisplayName("Data de Envio")]
        public DateTime SendData { get; set; }

        [ForeignKey("Client")]
        public int? ClientId { get; set; }

        [DisplayName("Cliente")]
        public Client? Client { get; set; }
        public int? InvoiceNumber { get; set; }

    }
}
