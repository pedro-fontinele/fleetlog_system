using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface ISmartgoImportationRepository
    {
        List<SmartgoImportation> GetByIdList(List<int> ids);
        List<SmartgoImportation> GetByDepositanteId(int depositanteId);

        void DeleteByDepositanteId(int depositanteId);

        List<SmartgoImportation> AddRange(List<SmartgoImportation> list);

    }
}
