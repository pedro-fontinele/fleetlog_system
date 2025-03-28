using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IExpeditionOrderTagShippingRepository
    {
        Task<ExpeditionOrderTagShipping> GetShippingByShippingCode(string order);
        Task<ExpeditionOrderTagShipping> UpdateAsync(ExpeditionOrderTagShipping orderShipping);
        ExpeditionOrderTagShipping Update(ExpeditionOrderTagShipping orderShipping);
        Task<ExpeditionOrderTagShipping> AddAsync(ExpeditionOrderTagShipping orderShipping);
        ExpeditionOrderTagShipping Add(ExpeditionOrderTagShipping orderShipping);
        Task<ExpeditionOrderTagShipping?> GetShippingByInvoiceAccessKeyAsync(string accessKey);
        Task<ExpeditionOrderTagShipping> GetShippingByExpeditionOrderIdAsync(int id);
        ExpeditionOrderTagShipping GetShippingByExpeditionOrderId(int id);
    }
}
