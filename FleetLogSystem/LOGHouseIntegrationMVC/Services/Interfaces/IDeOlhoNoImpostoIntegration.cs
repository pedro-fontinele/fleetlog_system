using LOGHouseSystem.Adapters.Extensions.DeOlhoNoImposto;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IDeOlhoNoImpostoIntegration
    {
        Task<DeOlhoNoImpostoResponse> GetTax(string codigo, string uf, string cnpj, string descricao, string unidadeMedida, string valor, string gtin);
    }
}
