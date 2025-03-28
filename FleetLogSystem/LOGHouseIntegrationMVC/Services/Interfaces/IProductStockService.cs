namespace LOGHouseSystem.Services.Interfaces
{
    public interface IProductStockService
    {
        void CreateHangFireProcessByClientId(int clientId, DateTime dateTime);
        Task ProcessByProductId(int productId, DateTime dateTime);
    }
}
