using LOGHouseSystem.Adapters.Extensions.ShopeeExtension.Dto;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services
{
    public interface IShopeeService
    {
        Task ProcessAllOrderDocuments(int clientId);
        Task DownloadPdfShippingFile(ShipmentListShiptShopeeDto order, int clientId, ExpeditionOrder expeditionOrder = null);
        Task DownloadPdfShippingFileByExpeditionOrder(ExpeditionOrder order);
        void CreateIntegrationVariables(int id1, int id2);
    }
}
