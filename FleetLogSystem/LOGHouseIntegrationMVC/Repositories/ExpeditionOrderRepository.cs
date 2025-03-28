using DocumentFormat.OpenXml.Spreadsheet;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Infra.Pagination;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LOGHouseSystem.Repositories
{
    public class ExpeditionOrderRepository : RepositoryBase, IExpeditionOrderRepository
    {
        private readonly IPickingListRepository _pickingListRepository;
        
        public ExpeditionOrderRepository(IPickingListRepository pickingRepository)
        {
            _pickingListRepository = pickingRepository;
        }

        public async Task<ExpeditionOrder> Add(ExpeditionOrder expeditionOrder)
        {
            //expeditionOrder.IssueDate = DateTime.Now;
            _db.ExpeditionOrders.Add(expeditionOrder);
            await _db.SaveChangesAsync();
            return expeditionOrder;
        }

        public ExpeditionOrder AddOrder(ExpeditionOrder expeditionOrder)
        {
            if(expeditionOrder.ClientId == null || expeditionOrder.ClientId == 0) 
            {
                User userLoged = _session.SearchUserSession();

                if (userLoged == null || userLoged.Id <= 0) 
                    throw new Exception("Erro ao buscar usuário");


                Client client = _db.Clients.FirstOrDefault(x => x.Id == userLoged.Id);

                expeditionOrder.ClientId = client.Id;
            }
            

            _db.ExpeditionOrders.Add(expeditionOrder);
            _db.SaveChanges();

            if(expeditionOrder.InvoiceAccessKey == "manual")
            {
                expeditionOrder.InvoiceAccessKey = $"PEDIDO - {expeditionOrder.Id}";
                Update(expeditionOrder);
            }
            return expeditionOrder;
        }

        public Task<ExpeditionOrder> GetOrderByIdAsync(int id)
        {
            return _db.ExpeditionOrders.Include(e => e.ExpeditionOrderTagShipping).Include(e => e.Client).Include(e => e.ExpeditionOrderItems).Include(e => e.ShippingDetails).Where(e => e.Id == id).FirstOrDefaultAsync();
        }

        public ExpeditionOrder GetById(int id)
        {
            return _db.ExpeditionOrders
                      .Include(x=> x.ExpeditionOrderItems)
                      .FirstOrDefault(x => x.Id == id);
        }



        public async Task<int> CoutItens(PaginationBase<ExpeditionOrder> pag)
        {
            return await _db.ExpeditionOrders
                      .CountAsync();
        }

        public ExpeditionOrder GetOrderByInvoiceAccessKey(string invoiceAccessKey)
        {
            return  _db.ExpeditionOrders.Where(e => e.InvoiceAccessKey == invoiceAccessKey).FirstOrDefault();
        }

        public async Task<ExpeditionOrder> GetOrderByInvoiceAccessKeyAsync(string invoiceAccessKey)
        {
            return await _db.ExpeditionOrders.Where(e => e.InvoiceAccessKey == invoiceAccessKey).FirstOrDefaultAsync();
        }

        public ExpeditionOrder Update(ExpeditionOrder order)
        {
            var entry = _db.Entry(order);

            if (entry.State == EntityState.Detached)
            {
                var set = _db.Set<ExpeditionOrder>();
                ExpeditionOrder attachedEntity = set.Find(order.Id);  // You need to have access to key

                if (attachedEntity != null)
                {
                    var attachedEntry = _db.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(order);
                }
                else
                {
                    entry.State = EntityState.Modified; // This should attach entity
                }
            }

            _db.SaveChanges();
            return order;
        }

        public async Task<List<ExpeditionOrder>> GetAllById(IEnumerable<int> ids)
        {
            return await _db.ExpeditionOrders.Include(e => e.ExpeditionOrderItems).Include(e => e.ShippingDetails).Include(e => e.ExpeditionOrderTagShipping).Include(e => e.Client).Where(e => ids.Any(item => item == e.Id)).ToListAsync();
        }

        public async Task<PaginationBase<ExpeditionOrder>> GetOrdersByClientIdAndbyStatusAsync(int id, List<ExpeditionOrderStatus> status, ExpeditionOrderFilterViewModel filter)
        {

            List<int> enumIntValues = status.Select(a => (int)a).ToList();

            //Gets all orders with status in the list and orders pending process from the customer panel
            var query = _db.ExpeditionOrders
                .Where(x => x.ClientId == id && enumIntValues.Contains((int)x.Status) || x.OrderOrigin == OrderOrigin.ClientPanel && x.Status == ExpeditionOrderStatus.ProcessingPendenting && x.ClientId == id)
                .Include(e => e.ExpeditionOrderTagShipping)
                .OrderByDescending(x => x.Id);

            return await PaginateQuery<ExpeditionOrder>(query, filter);

        }

        public async Task<List<ExpeditionOrder>> GetOrdersByClientIdAndbyStatusAsync(int id, List<ExpeditionOrderStatus> status)
        {

            List<int> enumIntValues = status.Select(a => (int)a).ToList();

            //Gets all orders with status in the list and orders pending process from the customer panel
            var query = _db.ExpeditionOrders
                .Where(x => x.ClientId == id && enumIntValues.Contains((int)x.Status))
                .Include(e => e.ExpeditionOrderTagShipping)
                .OrderByDescending(x => x.Id);

            return await query.ToListAsync();

        }

        public async Task<PaginationBase<ExpeditionOrder>> GetAllOrdersByStatusAsync(List<ExpeditionOrderStatus> status, ExpeditionOrderFilterViewModel filter)
        {
            List<int> enumIntValues = status.Select(a => (int)a).ToList();

            var query = _db.ExpeditionOrders
                .Where(x => enumIntValues.Contains((int)x.Status));

            if (status.Contains(ExpeditionOrderStatus.ProcessingPendenting))
            {
                query = query
                    .Where(x => x.OrderOrigin != OrderOrigin.ClientPanel && x.OrderOrigin != OrderOrigin.XMLCreation);
            }


            query = query
                .Include(e => e.ExpeditionOrderTagShipping)
                .Include(x => x.ShippingDetails)
                .OrderByDescending(x => x.Id);


            PaginationBase<ExpeditionOrder> pagination = new PaginationBase<ExpeditionOrder>();

            filter.PageSize = 100;
            pagination.PageNumber = filter.PageNumber;


            return await PaginateQuery<ExpeditionOrder>(query, filter);



        }

        public async Task<List<ExpeditionOrder>> GetOrdersByClientIdAndbyStatusIsNotAsync(int id, List<ExpeditionOrderStatus> status)
        {
            List<int> enumIntValues = status.Select(a => (int)a).ToList();

            return await _db.ExpeditionOrders
            .Where(x => x.ClientId == id && !enumIntValues.Contains((int)x.Status))
            .Include(e => e.ExpeditionOrderTagShipping)
            .ToListAsync();

                    
        }

        public async Task<List<ExpeditionOrder>> GetAllOrdersByStatusIsNotAsync(List<ExpeditionOrderStatus> status)
        {
            List<int> enumIntValues = status.Select(a => (int)a).ToList();

            return await _db.ExpeditionOrders
             .Where(x => !enumIntValues.Contains((int)x.Status))
             .Include(e => e.ExpeditionOrderTagShipping)
             .ToListAsync();
        }


        public async Task<List<ExpeditionOrder>> GetAllOrdersByStatusAndDateIsNotAsync(List<ExpeditionOrderStatus> status, DateTime date)
        {
            List<int> enumIntValues = status.Select(a => (int)a).ToList();

            return await _db.ExpeditionOrders
             .Where(x => !enumIntValues.Contains((int)x.Status) && x.IssueDate >= date)
             .Include(e => e.ExpeditionOrderTagShipping)
             .ToListAsync();
        }

        public async Task<ExpeditionOrder> GetByExternalNumber(ExpeditionOrder order, OrderOrigin origin)
        {
            return await _db.ExpeditionOrders.FirstOrDefaultAsync(x => x.ExternalNumber == order.ExternalNumber && x.ClientId == order.ClientId && x.OrderOrigin == origin);
        }


        public IQueryable<ExpeditionOrder> GetQueryableFilter(ExpeditionOrderFilterViewModel filter, List<ExpeditionOrderStatus> statusToFilter = null)
        {

            IQueryable<ExpeditionOrder> query;

            if (filter.Client != null)
            {
                query = _db.ExpeditionOrders
                   .Include(e => e.ExpeditionOrderTagShipping)
                   .Where(x => x.ClientId == filter.Client.Id)
                   .OrderByDescending(x => x.Id);                   
            }
            else
            {
                query = _db.ExpeditionOrders
                               .Include(e => e.ExpeditionOrderTagShipping)
                               .OrderByDescending(x => x.Id);
            }

            if (statusToFilter != null)
            {
                List<int> enumIntValues = statusToFilter.Select(a => (int)a).ToList();
                query = query.Where(x => x.Id > 0 && enumIntValues.Contains((int)x.Status));
            }

            if (!string.IsNullOrEmpty(filter.ClientName))
            {
                query = query.Where(x => x.ClientName.Contains(filter.ClientName));
            }

            if (filter.ClientId != null)
            {
                query = query.Where(x => x.ClientId == filter.ClientId);
            }

            if (!string.IsNullOrEmpty(filter.InvoiceNumber))
            {
                int invoiceNum = int.Parse(filter.InvoiceNumber);
                query = query.Where(x => x.InvoiceNumber == invoiceNum);
            }

            if (filter.OrderOrigin != null)
            {
                query = query.Where(x => x.OrderOrigin == filter.OrderOrigin);
            }

            if (filter.FinalizeStartDate != null && filter.FinalizeStartDate != new DateTime(1, 1, 1, 0, 0, 0))
            {
                query = query.Where(x => x.FinalizeDate.Value.Date >= filter.FinalizeStartDate.Value.Date);
            }

            if (filter.FinalizeEndDate != null && filter.FinalizeEndDate != new DateTime(1, 1, 1, 0, 0, 0))
            {
                query = query.Where(x => x.FinalizeDate.Value.Date <= filter.FinalizeEndDate.Value.Date);
            }

            if (filter.CreationStartDate != null && filter.CreationStartDate != new DateTime(1, 1, 1, 0, 0, 0))
            {
                query = query.Where(x => x.CreationDate.Value.Date >= filter.CreationStartDate.Value.Date);
            }

            if (filter.CreationEndDate != null && filter.CreationEndDate != new DateTime(1, 1, 1, 0, 0, 0))
            {
                query = query.Where(x => x.CreationDate.Value.Date <= filter.CreationEndDate.Value.Date);
            }

            if (filter.IssueDateStart != null)
            {
                query = query.Where(x => x.IssueDate.Value.Date >= filter.IssueDateStart.Value.Date);
            }

            if (filter.IssueDateEnd != null)
            {
                query = query.Where(x => x.IssueDate.Value.Date <= filter.IssueDateEnd.Value.Date);
            }

            if (!string.IsNullOrEmpty(filter.ShippingCompany))
            {
                query = query.Where(x => x.ShippingCompany.Contains(filter.ShippingCompany));
            }

            if (filter.ShippingMethod != null)
            {
                query = query.Where(x => x.ShippingMethod == filter.ShippingMethod);
            }

            if (filter.Status != null)
            {
                query = query.Where(x => x.Status == filter.Status);
            }


            if (filter.ShippingTagBlocked != null)
            {
                query = query.Where(x => x.ShippingTagBlocked == (filter.ShippingTagBlocked == 1 ? true : false));
            }

            if (!string.IsNullOrEmpty(filter.ExternalNumber))
            {
                query = query.Where(x => x.ExternalNumber.Contains(filter.ExternalNumber));
            }

            return query;
        }

        public async Task<PaginationBase<ExpeditionOrder>> GetByFilter(ExpeditionOrderFilterViewModel filter, List<ExpeditionOrderStatus> statusToFilter)
        {

            var query = GetQueryableFilter(filter, statusToFilter);


            return await PaginateQuery<ExpeditionOrder>(query, filter);

        }

        public ExpeditionOrder GetByInvoiceAccessKey(string invoiceAccessKey)
        {
            return _db.ExpeditionOrders.FirstOrDefault(x => x.InvoiceAccessKey == invoiceAccessKey);
        }

        public List<int> GetOrdersIdsByPickingList(int id)
        {
            return _db.ExpeditionOrders
                .AsNoTracking()
                .Where(e => e.PickingListId == id).Select(eo => eo.Id).ToList();
        }

        public async Task<List<ExpeditionOrder>> GetOrdersIsNotGeneratedReturnInvoice(DateTime maxDate)
        {
            var order = await _db.ExpeditionOrders                      
                      .Where(x => x.ReturnedInvoiceGenerated == YesOrNo.No && x.Status == ExpeditionOrderStatus.Dispatched && x.IssueDate <= maxDate)
                      .Include(x => x.ExpeditionOrderItems)
                      .ToListAsync();

            return order;
        }


        public async Task<List<ExpeditionOrder>> GetOrdersIsNotGeneratedReturnInvoiceByClientId(int clientId)
        {
            var order = await _db.ExpeditionOrders
                      .Where(x => x.ReturnedInvoiceGenerated == YesOrNo.No && x.Status == ExpeditionOrderStatus.Dispatched && x.ClientId == clientId)
                      .Include(x => x.ExpeditionOrderItems)
                      .ToListAsync();

            return order;
        }

        public List<ExpeditionOrder> GetByPackingListTransportationId(int packingListTransportationId)
        {
            return _db.ExpeditionOrders.Where(x => x.PackingListTransportationId == packingListTransportationId).ToList();
        }

        public async Task<List<ExpeditionOrder>> GetAllToday(DateTime now)
        {
            var dateStart = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            dateStart = dateStart.AddDays(-1);
            var dateEnd = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            dateEnd = dateEnd.AddDays(-1);
            
            return await _db.ExpeditionOrders.Where(x => x.IssueDate >= dateStart && x.IssueDate <= dateEnd).ToListAsync();            
        }

        public async Task<ExpeditionOrder> UpdateAsync(ExpeditionOrder order)
        {
            var entry = _db.Entry(order);

            if (entry.State == EntityState.Detached)
            {
                var set = _db.Set<ExpeditionOrder>();
                ExpeditionOrder attachedEntity = await set.FindAsync(order.Id);  // You need to have access to key

                if (attachedEntity != null)
                {
                    var attachedEntry = _db.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(order);
                }
                else
                {
                    entry.State = EntityState.Modified; // This should attach entity
                }
            }

            await _db.SaveChangesAsync();
            return order;
        }
        
        public async Task<List<int>> GetOrdersIdsByPickingListAsync(int id)
        {
            return await _db.ExpeditionOrders
                    .AsNoTracking()
                    .Where(e => e.PickingListId == id).Select(eo => eo.Id).ToListAsync();
        }


        public async Task<PaginationBase<ExpeditionOrder>> GetAllManualOrdersAsync(ExpeditionOrderFilterViewModel filter)
        {

            IQueryable<ExpeditionOrder> query = GetQueryableFilterToManualOrders(filter);

            PaginationBase<ExpeditionOrder> pagination = new PaginationBase<ExpeditionOrder>();

            filter.PageSize = 100;
            pagination.PageNumber = filter.PageNumber;


            return await PaginateQuery<ExpeditionOrder>(query, filter);
        }

        public IQueryable<ExpeditionOrder> GetQueryableFilterToManualOrders(ExpeditionOrderFilterViewModel filter)
        {

            IQueryable<ExpeditionOrder> query;

            query = _db.ExpeditionOrders
            .Where(x => x.Status == ExpeditionOrderStatus.ProcessingPendenting && x.OrderOrigin == OrderOrigin.ClientPanel || x.Status == ExpeditionOrderStatus.ProcessingPendenting && x.OrderOrigin == OrderOrigin.XMLCreation)
            .Include(e => e.ExpeditionOrderTagShipping)
            .Include(X => X.ShippingDetails)
            .OrderByDescending(x => x.Id);


            if (!string.IsNullOrEmpty(filter.ClientName))
            {
                query = query.Where(x => x.ClientName.Contains(filter.ClientName));
            }

            if (filter.ClientId != null)
            {
                query = query.Where(x => x.ClientId == filter.ClientId);
            }

            if (!string.IsNullOrEmpty(filter.InvoiceNumber))
            {
                int invoiceNum = int.Parse(filter.InvoiceNumber);
                query = query.Where(x => x.InvoiceNumber == invoiceNum);
            }

            if (filter.OrderOrigin != null)
            {
                query = query.Where(x => x.OrderOrigin == filter.OrderOrigin);
            }


            if (filter.CreationStartDate != null && filter.CreationStartDate != new DateTime(1, 1, 1, 0, 0, 0))
            {
                query = query.Where(x => x.CreationDate.Value.Date >= filter.CreationStartDate.Value.Date);
            }

            if (filter.CreationEndDate != null && filter.CreationEndDate != new DateTime(1, 1, 1, 0, 0, 0))
            {
                query = query.Where(x => x.CreationDate.Value.Date <= filter.CreationEndDate.Value.Date);
            }

            if (filter.IssueDateStart != null)
            {
                query = query.Where(x => x.IssueDate.Value.Date >= filter.IssueDateStart.Value.Date);
            }

            if (filter.IssueDateEnd != null)
            {
                query = query.Where(x => x.IssueDate.Value.Date <= filter.IssueDateEnd.Value.Date);
            }

            if (!string.IsNullOrEmpty(filter.ShippingCompany))
            {
                query = query.Where(x => x.ShippingCompany.Contains(filter.ShippingCompany));
            }

            if (filter.ShippingMethod != null)
            {
                query = query.Where(x => x.ShippingMethod == filter.ShippingMethod);
            }


            return query;

        }

        public async Task<ExpeditionOrder> GetOrderByInvoiceNumberAndClientId(int id, int invoiceNumber, int clientId)
        {
            return await _db.ExpeditionOrders.FirstOrDefaultAsync(x => x.InvoiceNumber == invoiceNumber && x.ClientId == clientId && x.Id != id);
        }
    }
}
