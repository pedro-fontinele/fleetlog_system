namespace LOGHouseSystem.ViewModels.ReceiptNote
{
    public class SetPositionToItemsViewModel
    {
        public IEnumerable<ReceiptNoteWithPositionViewModel> Products { get; set; }

        public int ReceiptNoteID { get; set; }
    }
}
