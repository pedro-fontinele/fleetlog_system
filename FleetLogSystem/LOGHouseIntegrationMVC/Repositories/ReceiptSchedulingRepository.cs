using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class ReceiptSchedulingRepository : RepositoryBase, IReceiptSchedulingRepository
    {

        public IEnumerable<ReceiptScheduling> GetAll()
        {
           return _db.ReceiptSchedulings
                .AsNoTracking()
                .OrderByDescending(r=>r.Id)
                .Include(x => x.Client)
                .ToList();
        }

        public ReceiptScheduling GetById(int id)
        {
            return _db.ReceiptSchedulings
                      .FirstOrDefault(x => x.Id == id);
        }

        public ReceiptScheduling Add(ReceiptScheduling receiptScheduling)
        {
            if (receiptScheduling == null) throw new ArgumentNullException();

            _db.ReceiptSchedulings.Add(receiptScheduling); 
            _db.SaveChanges();

            return receiptScheduling;
        }


        public ReceiptScheduling UpdateYesOrNoStatus(int id, YesOrNo status)
        {
            

            ReceiptScheduling receiptSchedulingById = GetById(id);

            if (receiptSchedulingById == null) throw new ArgumentNullException();

            receiptSchedulingById.GeneratedReceiptNote = status;


            _db.ReceiptSchedulings.Update(receiptSchedulingById);
            _db.SaveChanges();

            return receiptSchedulingById;
        }
    }
}
