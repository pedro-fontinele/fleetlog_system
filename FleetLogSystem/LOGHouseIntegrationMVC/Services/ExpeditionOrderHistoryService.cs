using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services
{
    public class ExpeditionOrderHistoryService : IExpeditionOrderHistoryService
    {
        private readonly IExpeditionOrderHistoryRepository _expeditionOrderHistoryRepository;
        private readonly IUserRepository _userRepository;

        public ExpeditionOrderHistoryService(IExpeditionOrderHistoryRepository expeditionOrderHistoryRepository, IUserRepository userRepository)
        {
            _expeditionOrderHistoryRepository = expeditionOrderHistoryRepository;
            _userRepository = userRepository;

        }
        public async Task Add(int? orderId, string? obs, ExpeditionOrderStatus status, int userId = 0)
        {
            int userLogedId = 0;

            if(userId > 0)
            {
                userLogedId = userId;
            }
            else
            {
                 userLogedId = _userRepository.GetUserLoged().Id;
            }

            ExpeditionOrderHistory history = new ExpeditionOrderHistory()
            {
                UserId = userLogedId,
                ExpeditionOrderId = orderId,
                Observation = obs,
                Status = status,
                Date = DateTimeHelper.GetCurrentDateTime()
            };

           await _expeditionOrderHistoryRepository.Add(history);
        }
        public void AddNotAsync(int? orderId, string? obs, ExpeditionOrderStatus status, int userId = 0)
        {
            int userLogedId = 0;

            if (userId > 0)
            {
                userLogedId = userId;
            }
            else
            {
                userLogedId = _userRepository.GetUserLoged().Id;
            }

            ExpeditionOrderHistory history = new ExpeditionOrderHistory()
            {
                UserId = userLogedId,
                ExpeditionOrderId = orderId,
                Observation = obs,
                Status = status,
                Date = DateTimeHelper.GetCurrentDateTime()
            };

             _expeditionOrderHistoryRepository.AddNotAsync(history);
        }


        public Task<List<ExpeditionOrderHistory>> GetByOrderId(int id)
        {
            return _expeditionOrderHistoryRepository.GetByOrderIdAsync(id);
        }
    }
}
