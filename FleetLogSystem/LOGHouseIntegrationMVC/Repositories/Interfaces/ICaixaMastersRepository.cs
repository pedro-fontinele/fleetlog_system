using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface ICaixaMastersRepository
    {
        CaixaMaster Add(CaixaMaster product);
        CaixaMaster? GetByCode(string code);
        string GetCodeByReceiptNoteItemId(int id);
    }
}
