using LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Dto;
using LOGHouseSystem.Adapters.Extensions.SmartgoExtension.Responses;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services.SmartgoImportation
{
    public class SmartgoImportationService : ISmartgoImportationService
    {
        private readonly ISmartgoImportationRepository _smartgoImportationRepository;

        public SmartgoImportationService(ISmartgoImportationRepository smartgoImportationRepository)
        {
            _smartgoImportationRepository = smartgoImportationRepository;
        
        }

        public List<Models.SmartgoImportation> GenerateSmartgoImportation(int depositanteId, List<SaldoDetalhado> saldoDetalhado)
        {
            _smartgoImportationRepository.DeleteByDepositanteId(depositanteId);

            var addedItems = _smartgoImportationRepository.AddRange(saldoDetalhado.Select(es => new Models.SmartgoImportation
            {
                EAN = (es.ProdutoCodigoExterno ?? "").Trim(),
                IdDepositante = depositanteId,
                Lote = es.Lote,
                PositionAddress = (es.EstruturaCodigo ?? "").Trim(),
                ProductName = es.ProdutoNome,
                Quantity = es.QuantidadeDisponivel,
                SKU = (es.ProdutoCodigoInterno ?? "").Trim(),
                UnitPrice = es.ValorUnitario,
                Validade = es.DataDeValidade
            }).ToList());

            return addedItems;
        }

        public List<Models.SmartgoImportation> GetItemsByIdList(int depositanteId, int[] ids)
        {
            var items = _smartgoImportationRepository.GetByDepositanteId(depositanteId);

            return items.Where(i => ids.Contains(i.Id)).ToList();
        }

    }
}
