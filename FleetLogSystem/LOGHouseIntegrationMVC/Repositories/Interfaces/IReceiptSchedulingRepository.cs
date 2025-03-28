using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IReceiptSchedulingRepository
    {
        IEnumerable<ReceiptScheduling> GetAll();

        ReceiptScheduling GetById(int id);

        ReceiptScheduling Add(ReceiptScheduling receiptScheduling);

        ReceiptScheduling UpdateYesOrNoStatus(int id, YesOrNo status);
    }
}
