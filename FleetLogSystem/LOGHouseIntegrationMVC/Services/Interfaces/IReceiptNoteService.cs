using LOGHouseSystem.Controllers;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IReceiptNoteService
    {
        ResponseDTO ConfirmReceiptNote(int id);
        ResponseDTO RejectReceiptNote(int id, string subject, string? message);
        ResponseDTO ResetReceiptNote(int id);
        List<ReceiptNoteDashboardViewModel> ReceiptNoteToDashboard();
        ReceiptNoteDashboardViewModel ReceiptNoteToDashboardMapper(ReceiptNote note);
        int getPercent(ReceiptNote note);
        bool DeleteById(int id);
        string GetReceiptNoteNumberByAcessKey(string acessKey);
    }
}
