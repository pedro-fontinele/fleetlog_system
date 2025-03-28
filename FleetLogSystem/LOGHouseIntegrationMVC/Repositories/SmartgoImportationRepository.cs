using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;

namespace LOGHouseSystem.Repositories
{
    public class SmartgoImportationRepository : RepositoryBase, ISmartgoImportationRepository
    {
        public List<SmartgoImportation> AddRange(List<SmartgoImportation> list)
        {
            _db.SmartgoImportations.AddRange(list);
            _db.SaveChanges();
            return list;
        }

        public void DeleteByDepositanteId(int depositanteId)
        {
            var items = _db.SmartgoImportations.Where(si => si.IdDepositante == depositanteId).ToList();
            if (items.Count > 0)
            {
                _db.SmartgoImportations.RemoveRange(items);
                _db.SaveChanges();
            }
        }

        public List<SmartgoImportation> GetByDepositanteId(int depositanteId)
        {
            return _db.SmartgoImportations.Where(si => si.IdDepositante == depositanteId).ToList();
        }

        public List<SmartgoImportation> GetByIdList(List<int> ids)
        {
            return _db.SmartgoImportations.Where(si => ids.Contains(si.Id)).ToList();
        }
    }
}
