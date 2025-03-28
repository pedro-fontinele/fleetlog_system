using LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Dto;
using LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Responses;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface ISmartgoImportationService
    {
        List<Models.SmartgoImportation> GenerateSmartgoImportation(int depositanteId, List<SaldoDetalhado> saldoDetalhado);

        List<Models.SmartgoImportation> GetItemsByIdList(int depositanteId, int[] ids);

    }
}
