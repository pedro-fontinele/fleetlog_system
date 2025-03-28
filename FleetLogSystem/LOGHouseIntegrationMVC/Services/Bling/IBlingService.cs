using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Services
{
    public interface IBlingService
    {
        Task IntegrateOrders(OrderIntegrationViewModel orderSearch);
    }
}
