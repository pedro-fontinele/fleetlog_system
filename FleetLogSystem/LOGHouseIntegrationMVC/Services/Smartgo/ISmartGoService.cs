using LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Dto;
using LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Responses;

namespace LOGHouseSystem.Services.Smartgo
{
    public interface ISmartGoService
    {
        public Task<List<EstoqueSimplificado>> GetSimplifiedStockByDepositorId(int depositorId);
        public Task<List<SaldoDetalhado>> GetSaldoDetalhadoByDepositorId(int depositorId);
    }
}
