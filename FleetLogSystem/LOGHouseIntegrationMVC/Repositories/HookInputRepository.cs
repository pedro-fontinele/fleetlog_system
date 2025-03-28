using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class HookInputFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class HookInputRepository : RepositoryBase, IHookInputRepository
    {
        public async Task<HookInput> Add(HookInput hook)
        {
            await _db.HookInputs.AddAsync(hook);
            await _db.SaveChangesAsync();

            return hook;
        }

        public async Task<HookInput> GetById(int id)
        {
         return await _db.HookInputs.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> Delete(HookInput hook)
        {
            _db.HookInputs.Remove(hook);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<HookInput> Update(HookInput hook)
        {
            HookInput hookById = await GetById(hook.Id);

            hookById.Note = hook.Note;
            hookById.Payload = hook.Payload;
            hookById.Cnpj = hook.Cnpj;
            hookById.Origin = hook.Origin;

            _db.HookInputs.Update(hookById);
            await _db.SaveChangesAsync();

            return hookById;
        }

        public async Task<List<HookInput>> GetAll()
        {
            return await _db.HookInputs.Where(e => e.Status == true).ToListAsync();
        }

        public async Task<List<HookInput>> GetAllByType(HookTypeEnum type)
        {
            return await _db.HookInputs.Where(e => e.Status == true).Where(e => e.Type == type).ToListAsync();
        }

        public async Task<HookInput> GetByCodeAndCnpj(string code, string cnpj)
        {
            return await _db.HookInputs.Where(e => e.Code == code && e.Cnpj == cnpj).FirstOrDefaultAsync();
        }

        public IQueryable<HookInput> GetHookInputs(HookInputFilter filter = null)
        {
            var query = _db.HookInputs.Select(hi => hi);

            if(filter != null)
            {
                if(filter.EndDate != null) query = query.Where(hi => hi.Date <= filter.EndDate);
                if(filter.StartDate != null) query = query.Where(hi => hi.Date >= filter.StartDate);
            }

            return query;
        }
    }
}
