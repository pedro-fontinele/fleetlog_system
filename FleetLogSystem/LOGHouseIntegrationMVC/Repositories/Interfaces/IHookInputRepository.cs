using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;

namespace LOGHouseSystem.Repositories.Interfaces
{
    public interface IHookInputRepository
    {
        Task<HookInput> Add(HookInput hook);

        Task<HookInput> GetById(int id);

        Task<bool> Delete(HookInput hook);

        Task<HookInput> Update(HookInput hook);

        Task<List<HookInput>> GetAll();
        Task<List<HookInput>> GetAllByType(HookTypeEnum type);
        Task<HookInput> GetByCodeAndCnpj(string code, string cnpj);

        public IQueryable<HookInput> GetHookInputs(HookInputFilter filter);
    }
}
