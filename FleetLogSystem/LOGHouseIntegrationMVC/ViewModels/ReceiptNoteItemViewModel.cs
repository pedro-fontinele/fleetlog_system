using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LOGHouseSystem.ViewModels
{
    public class ReceiptNoteItemViewModel
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

        [DisplayName("Quantidade Recebida")]
        public double QuantityInspection { get; set; }

        [DisplayName("Valor Unitario")]
        public decimal? Value { get; set; }

        [DisplayName("Status Item")]
        public NoteItemStatus ItemStatus { get; set; }

        public string CaixaMasterCode { get; set; }

        public int ReceiptNoteId { get; set; }
    }
}
