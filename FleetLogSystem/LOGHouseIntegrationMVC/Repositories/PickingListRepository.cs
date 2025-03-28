using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using PagedList;

namespace LOGHouseSystem.Repositories
{
    public class PickingListRepository : RepositoryBase, IPickingListRepository
    {

        public async Task<PickingList> GetByOrderExpeditionIdAsync(int id)
        {
            return await _db.PickingLists
                .AsNoTracking()
                .Include(x => x.ExpeditionOrder)
                .FirstOrDefaultAsync(x => x.ExpeditionOrder.Any(e => e.Id == id));
                
        }

        public List<PickingList> GetByStatus(PickingListStatus status)
        {
            return _db.PickingLists
                .Where(x => x.Status == status)
                .AsNoTracking()
                .Include(x => x.PickingListItems)                
                .Include(x => x.ExpeditionOrder)
                .ToList();
        }


        public List<PickingList> GetAllWithStatusGeradoAndEmAtendimento()
        {
            return _db.PickingLists
                .Where(x => x.Status == PickingListStatus.EmAtendimento || x.Status == PickingListStatus.Gerado)
                .AsNoTracking()
                .Include(x => x.PickingListItems)                
                .Include(x => x.ExpeditionOrder)
                .OrderByDescending(x=>x.Priority)
                .ToList();
        }
        public async Task<PickingList> Create(PickingList picking)
        {
            picking.CreatedAt = DateTimeHelper.GetCurrentDateTime();
            _db.PickingLists.Add(picking);
            await _db.SaveChangesAsync();

            return picking;

        }

        public PickingList GetById(int id)
        {
            return _db.PickingLists
                .Include(x => x.PickingListItems)
                .ThenInclude(x => x.Product)                
                .Include(x => x.ExpeditionOrder)
                .ThenInclude(x => x.Client)
                .FirstOrDefault(x => x.Id == id);
        }

        public PickingList Update(PickingList pickingList)
        {

            var entry = _db.Entry(pickingList);

            if (entry.State == EntityState.Detached)
            {
                var set = _db.Set<PickingList>();
                PickingList attachedEntity = set.Find(pickingList.Id);  // You need to have access to key

                if (attachedEntity != null)
                {
                    var attachedEntry = _db.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(pickingList);
                }
                else
                {
                    entry.State = EntityState.Modified; // This should attach entity
                }
            }

            _db.SaveChanges();
            return pickingList;
        }

        public bool CancelById(int id)
        {
           

            PickingList picking = GetById(id);

            picking.Status = PickingListStatus.Cancelado;

            Update(picking);

            return true;
        }

        public async Task<PickingList> GetByIdAsync(int? id)
        {
            return await _db.PickingLists
                .Include(x => x.PickingListItems)
                .ThenInclude(x => x.Product)                
                .Include(x => x.ExpeditionOrder)
                .ThenInclude(e => e.ShippingDetails)
                .Include(pl => pl.ExpeditionOrder)
                .ThenInclude(x => x.ExpeditionOrderTagShipping)
                .Include(pl => pl.ExpeditionOrder)
                .ThenInclude(x => x.Client)
                .Include(e => e.ExpeditionOrder)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public PagedList<PickingListWithUrlAndArrayByteViewModel> GetAllPaged(int page, int pageSize, int? cartId, int? invoiceNumber)
        {
            var query = _db.PickingLists
                      .AsNoTracking()
                      .Where(x => x.Status != PickingListStatus.Cancelado)
                      .Include(x => x.PickingListItems)
                      .Include(x => x.Cart)
                      .Include(x => x.ExpeditionOrder)
                      .ThenInclude(x => x.Client)
                      .Select(pl=>pl);

            List< PickingListWithUrlAndArrayByteViewModel > list = new List<PickingListWithUrlAndArrayByteViewModel> ();



            if (cartId != null && cartId > 0) query = query.Where(pl => pl.CartId == cartId);

            if (invoiceNumber != null && invoiceNumber > 0) query = query.Where(pl => pl.ExpeditionOrder.Any(a => a.InvoiceNumber == invoiceNumber));

            foreach (var item in query)
            {
                PickingListWithUrlAndArrayByteViewModel viewModel = new PickingListWithUrlAndArrayByteViewModel()
                {
                    Id = item.Id,
                    Responsible = item.Responsible,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    Status = item.Status,
                    CreatedAt = item.CreatedAt,
                    Priority = item.Priority,
                    MarketPlace = item.MarketPlace,
                    PickingListItems = item.PickingListItems,
                    ExpeditionOrder = item.ExpeditionOrder,
                    CartId = item.CartId,
                    Cart = item.Cart
                };

                list.Add(viewModel);
            }

            return (PagedList<PickingListWithUrlAndArrayByteViewModel>)list
                      .OrderByDescending(x => x.Id)
                      .ToPagedList(page, pageSize);
        }

        public PickingList GetLastByCartId(int cartId)
        {
            var pickingListNotPacked = _db.PickingLists
                .Include(x => x.PickingListItems)
                .ThenInclude(x => x.Product)
                .Include(x => x.ExpeditionOrder)
                .ThenInclude(e => e.ShippingDetails)
                .Include(pl => pl.ExpeditionOrder)
                .ThenInclude(x => x.ExpeditionOrderTagShipping)
                .Include(pl => pl.ExpeditionOrder)
                .ThenInclude(x => x.Client)
                .Include(e => e.ExpeditionOrder)
                .Where(e => e.ExpeditionOrder.First().Status < ExpeditionOrderStatus.Packed)
                .OrderByDescending(e => e.Id)
                .FirstOrDefault(x => x.CartId == cartId);

            if (pickingListNotPacked != null) return pickingListNotPacked;

            return _db.PickingLists
                .Include(x => x.PickingListItems)
                .ThenInclude(x => x.Product)
                .Include(x => x.ExpeditionOrder)
                .ThenInclude(e => e.ShippingDetails)
                .Include(pl => pl.ExpeditionOrder)
                .ThenInclude(x => x.ExpeditionOrderTagShipping)
                .Include(pl => pl.ExpeditionOrder)
                .ThenInclude(x => x.Client)
                .Include(e => e.ExpeditionOrder)
                .OrderByDescending(e => e.Id)
                .FirstOrDefault(x => x.CartId == cartId);
        }
    }
}

