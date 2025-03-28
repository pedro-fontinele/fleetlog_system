using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services
{
    public class PickingListHistoryService : IPickingListHistoryService
    {
        private readonly IPickingListHistoryRepository _pickingListHistoryRepository;
        private readonly IUserRepository _userRepository;

        public PickingListHistoryService(IPickingListHistoryRepository pickingListHistoryRepository, IUserRepository userRepository)
        {
            _pickingListHistoryRepository = pickingListHistoryRepository;
            _userRepository = userRepository;

        }
        public async Task Add(int pickingId, string? obs, PickingListStatus status, int userId = 0)
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

            PickingListHistory history = new PickingListHistory()
            {
                UserId = userLogedId,
                PickingListId = pickingId,
                Observation = obs,
                Status = status,
                Date = DateTime.Now
            };

           await _pickingListHistoryRepository.Add(history);
        }

        public async Task<List<PickingListHistory>> GetByPickingId(int pickingId)
        {
            return await _pickingListHistoryRepository.GetByPickingId(pickingId);
        }
    }
}
