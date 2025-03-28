using LOGHouseSystem.Adapters.Extensions.SmartgoExtension;
using LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Dto;
using LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Responses;

namespace LOGHouseSystem.Services.Smartgo
{
    public class SmartGoService : ISmartGoService
    {
        private readonly SmartgoExtension _smartgoExtension;

        public SmartGoService()
        {
            _smartgoExtension = new SmartgoExtension();
        }

        public async Task<List<SaldoDetalhado>> GetSaldoDetalhadoByDepositorId(int depositorId)
        {
            SaldoDetalhadoResponse response = await _smartgoExtension.GetSaldoDetalhadoPorDepositante(depositorId);
            List<SaldoDetalhado> StockItems = response.Model;

            return StockItems;
        }

        public async Task<List<EstoqueSimplificado>> GetSimplifiedStockByDepositorId(int depositorId)
        {
            int page = 0;
            SaldoSimplificadoResponse response;
            List<EstoqueSimplificado> StockItems = new List<EstoqueSimplificado>();
            do
            {
                page++;

                response = await _smartgoExtension.GetSaldoSimlpificadoPorDepositante(depositorId);
                if (response.Success)
                    StockItems.AddRange(response.Model.Items);

            } while (page <= response.Model.Page);

            return StockItems;
        }
    }
}
