using LOGHouseSystem.Infra.Enums;

namespace LOGHouseSystem.Controllers.API.BarcodeColectorApi
{
    public class BarcodeColectorFilter
    {
        public int? Id { get; set; }
        public int? ClientId { get; set; }
        public NoteStatus? NoteStatus { get; set; }
        public DateTime? ReceiptDate { get; set; }
    }
}
