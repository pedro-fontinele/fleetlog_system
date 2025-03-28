using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;

namespace LOGHouseSystem.Repositories
{
    public class CaixaMasterRepository : RepositoryBase, ICaixaMastersRepository
    {


        public CaixaMaster Add(CaixaMaster caixaMaster)
        {
            _db.CaixaMasters.Add(caixaMaster);
            _db.SaveChanges();
            return caixaMaster;
        }

        public CaixaMaster? GetByCode(string code)
        {
            return _db.CaixaMasters.FirstOrDefault(caixaMaster => caixaMaster.Code == code);
        }

        public string GetCodeByReceiptNoteItemId(int id)
        {
           CaixaMaster cm = _db.CaixaMasters.FirstOrDefault(x => x.ReceiptNoteItemId == id);

           if(cm == null)
            {
                return "-";
            }
            else
            {
                return cm.Code;
            }
        }
    }
}
