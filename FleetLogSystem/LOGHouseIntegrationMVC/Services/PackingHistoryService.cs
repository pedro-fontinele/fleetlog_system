using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services
{
    public class PackingHistoryService : IPackingHistoryService
    {
        private readonly IPackingHistoryRepository _packingHistoryRepository;
        private readonly IUserRepository _userRepository;

        public PackingHistoryService(IPackingHistoryRepository packingHistoryRepository, IUserRepository userRepository)
        {
            _packingHistoryRepository = packingHistoryRepository;
            _userRepository = userRepository;

        }
        public async Task Add(int packingId, string? obs, PackingStatus status, int userId = 0)
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

            PackingHistory history = new PackingHistory()
            {
                UserId = userLogedId,
                PackingId = packingId,
                Observation = obs,
                Status = status,
                Date = DateTime.Now
            };

           await _packingHistoryRepository.Add(history);
        }
    }
}
