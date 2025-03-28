using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services.Interfaces;

namespace LOGHouseSystem.Services
{
    public class HookInputService : IHookInputService
    {
        private readonly IHookInputRepository _hookInputRepository;
        public HookInputService(IHookInputRepository hookInputRepository)
        {
            _hookInputRepository = hookInputRepository;
        }

        public async Task<HookInput> ActivateHookByCodeAndCnpj(string code, string cnpj)
        {
            HookInput hook = await _hookInputRepository.GetByCodeAndCnpj(code, cnpj);

            if(hook != null) {
                hook.Status = true;
                await _hookInputRepository.Update(hook);
            }

            return hook;
        }

        public async Task<HookInput> Add(string data, OrderOrigin order, string? cnpj)
        {
            HookInput hook = new()
            {
                Payload = data,
                Origin = order
            };

            if(cnpj != null) 
                hook.Cnpj = cnpj;

           return await _hookInputRepository.Add(hook);

        }

        public async Task<HookInput> Add(HookInput data)
        {           
            return await _hookInputRepository.Add(data);
        }

        public async Task<bool> AddNoteById(int id, string note)
        {
            HookInput hook = await _hookInputRepository.GetById(id);

            if(hook == null)
            {
                throw new Exception("Erro ao atualizar HookInput");
            }

            hook.Note = note;

           HookInput hookUptade =  await _hookInputRepository.Update(hook);

            if (hookUptade != null) return true;

            throw new Exception("Erro ao atualizar HookInput");
        }

        public async Task DeleteByCodeAndCnpj(string code, string cnpj)
        {
            HookInput hook = await _hookInputRepository.GetByCodeAndCnpj(code, cnpj);

            if (hook != null)
            {
                await _hookInputRepository.Delete(hook);
            }
            
        }

        public async Task<bool> DeleteById(int id)
        {
            HookInput hook = await _hookInputRepository.GetById(id);

            if (hook != null)
            {
                return await _hookInputRepository.Delete(hook);
            }

            return false;
        }

        public async Task<List<HookInput>> GetAll()
        {
           List<HookInput> list =  await _hookInputRepository.GetAll();

            if (list == null || list.Count == 0)
                return null;

            return list;
        }

        public async Task<List<HookInput>> GetAllByType(HookTypeEnum type)
        {
            List<HookInput> list = await _hookInputRepository.GetAllByType(type);

            if (list == null || list.Count == 0)
                return null;

            return list;
        }

        public async Task InativateHook(HookInput order)
        {
            order.Status = false;
            await _hookInputRepository.Update(order);
        }

        public async Task UpdateCode(HookInput order, string code)
        {
            order.Code = code;
            await _hookInputRepository.Update(order);
        }

        public void DeleteOldHooks()
        {
            var hooksToDelete = _hookInputRepository.GetHookInputs(new Repositories.HookInputFilter
            {
                EndDate = DateTime.Now.AddDays(-7)
            }).ToList();

            foreach(HookInput hook in hooksToDelete)
            {
                _hookInputRepository.Delete(hook);
            }
        }
    }
}
