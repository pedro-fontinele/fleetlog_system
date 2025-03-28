using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Services.Interfaces
{
    public interface IHookInputService
    {
        //Task<HookInput> Add(string data, OrderOrigin origin, string? cnpj);
        //Task<HookInput> Add(HookInput hookInput);

        Task<bool> DeleteById(int id);

        Task<bool> AddNoteById(int id, string note);

        Task<List<HookInput>> GetAll();
        Task<List<HookInput>> GetAllByType(HookTypeEnum pedido);        
        Task InativateHook(HookInput order);
        Task<HookInput> ActivateHookByCodeAndCnpj(string code, string cnpj);
        Task DeleteByCodeAndCnpj(string code, string cnpj);
        Task UpdateCode(HookInput order, string code);

        public void DeleteOldHooks();
    }
}
