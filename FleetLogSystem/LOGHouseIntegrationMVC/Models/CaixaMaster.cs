namespace LOGHouseSystem.Models
{
    public class CaixaMaster
    { 
        public int Id { get; set; }   

        public string Code { get; set; }

        public DateTime CreatedAt { get; set; }

        public int Quantity { get; set; }

        public int ReceiptNoteItemId { get; set; }

        public virtual ReceiptNoteItem ReceiptNoteItem { get; set; }

    }
}
