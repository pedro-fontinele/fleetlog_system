using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.Controllers.API.BarcodeColectorApi.Response
{
    public class NoteCustomer
    {
        [DisplayName("CNPJ")]
        public string Cnpj { get; set; }

        [DisplayName("Razão Social")]
        [Required(ErrorMessage = "É necessário informar a Razão Soicial")]
        public string SocialReason { get; set; }
    }

    public class GetNotesByStatusResponse
    {
        public int Id { get; set; }

        [DisplayName("Número Nota")]
        public string Number { get; set; }

        [DisplayName("Data de Entrada")]
        public DateTime? EntryDate { get; set; }

        [DisplayName("Status")]
        public NoteStatus Status { get; set; }

        public int ClientId { get; set; }

        public virtual NoteCustomer Client { get; set; }

        public decimal ValidationPercent { get; set; }

    }
}
