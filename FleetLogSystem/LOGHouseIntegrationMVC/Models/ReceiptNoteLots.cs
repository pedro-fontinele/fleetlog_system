using LOGHouseSystem.Infra.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGHouseSystem.Models
{
    public class ReceiptNoteLots
    {
        public ReceiptNoteLots(int product, int receiptNoteId, double inputQuantity, double? outputQuantity)
        {
            ProductId = product;
            ReceiptNoteId = receiptNoteId;
            InputQuantity = inputQuantity;
            OutputQuantity = outputQuantity ?? 0;
            Status = LotStatus.Gerado;
        }

        public ReceiptNoteLots()
        {

        }

        [Key]
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int? ProductId { get; set; }

        public virtual Product Product { get; set; }

        [ForeignKey("ReceiptNote")]
        public int ReceiptNoteId { get; set; }

        public virtual ReceiptNote ReceiptNote { get; set; }

        [DisplayName("Data de Atualização")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [DisplayName("Quantidade de entrada")]
        public double InputQuantity { get; set; }

        [DisplayName("Quantidade de saida")]
        public double OutputQuantity { get; set; }
        public LotStatus Status { get; set; }

        public double CalculateOutput(double? newQuantity)
        {
            return this.OutputQuantity + newQuantity ?? 0;
        }

        public double Diference()
        {
            return InputQuantity - OutputQuantity;
        }
    }
}
