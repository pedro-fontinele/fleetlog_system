using LOGHouseSystem.Adapters.API.MercadoLivre.Response;
using RestSharp;

namespace LOGHouseSystem.Adapters.API.MercadoLivre
{
    public interface IMercadoLivreExtension
    {
        Task<RestResponse> GetShippingId(string orderId);
        Task<RestResponse> GetLabel(string shippmentId);
        Task<RestResponse> GetLabelPdf(string shippmentId);
        void SetMercadoLivreConnection(string accessToken, int mlUserId);
        Task<RestResponse> RefreshAccessToken(string refreshToken);
    }
}
