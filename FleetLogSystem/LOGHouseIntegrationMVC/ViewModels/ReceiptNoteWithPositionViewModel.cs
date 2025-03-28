using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.ViewModels
{
    public class ReceiptNoteWithPositionViewModel
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Código Produto")]
        public string Code { get; set; }

        [DisplayName("Descrição")]
        public string? Description { get; set; }

        [DisplayName("Código EAN")]
        public string? Ean { get; set; }

        [DisplayName("Quantidade")]
        public double Quantity { get; set; }

        [DisplayName("Status Item")]
        public NoteItemStatus ItemStatus { get; set; }

        public string? Position { get; set; }


    }
}
