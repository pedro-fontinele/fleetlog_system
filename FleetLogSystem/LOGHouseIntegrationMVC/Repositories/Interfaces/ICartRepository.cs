using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> AddAsync(Cart cart);
        Task<Cart> UpdateAsync(Cart cart);
        Task DeleteAsync(Cart cart);
        Task<Cart> GetByIdAsync(int id);
        Task<List<Cart>> GetAllAsync();
        Task<Cart> UpdateByIdAsync(int id, Cart newCart);
        byte[] GeneratePdfWithLabels(List<CartIdViewModel> itens);
    }
}
