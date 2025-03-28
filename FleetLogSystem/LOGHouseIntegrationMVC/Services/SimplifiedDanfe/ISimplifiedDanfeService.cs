using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services
{
    public interface ISimplifiedDanfeService
    {
        Task<string> GenerateSimplifiedDanfeByReceiptNote(string danfeBaseFileContent, Invoice receiptNote);

        string GenerateSimplifiedDanfe(string danfeBaseFileContent, ExpeditionOrder expeditionOrder, Client client);
    }
}
