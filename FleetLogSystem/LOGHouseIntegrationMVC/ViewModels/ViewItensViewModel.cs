using LOGHouseSystem.Models;

namespace LOGHouseSystem.ViewModels
{
    public class ViewItensViewModel
    {
        public List<ReceiptNoteItemViewModel> Itens { get; set; }

        public User UserLoged { get; set; }

        public string Url { get; set; } 
    }
}
