using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;

namespace LOGHouseSystem.Controllers.API.BarcodeColectorApi.Response
{
    public class GetPackingsByStatusResponse
    {
        public int Id { get; set; }

        [DisplayName("Responsável")]
        public string Responsible { get; set; }

        [DisplayName("Descrição")]
        public string Description { get; set; }

        [DisplayName("Quantidade")]
        public decimal? Quantity { get; set; }

        [DisplayName("Status")]
        public PackingStatus Status { get; set; }

        public int? ClientId { get; set; }

        public virtual NoteCustomer Client { get; set; }

        [DisplayName("Data de Criação")]
        public DateTime CreatedAt { get; set; }
    }
}
