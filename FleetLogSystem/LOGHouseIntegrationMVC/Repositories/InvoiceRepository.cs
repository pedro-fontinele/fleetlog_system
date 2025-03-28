using LOGHouseSystem.Controllers.API.BarcodeColectorApi;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Infra.Helpers;
using LOGHouseSystem.Models;
using LOGHouseSystem.Repositories.Interfaces;
using LOGHouseSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LOGHouseSystem.Repositories
{
    public class InvoiceRepository : RepositoryBase, IInvoiceRepository
    {

        public Invoice GetById(int id)
        {
            return _db.Invoices
                .Include(x => x.ExpeditionOrder)
                .FirstOrDefault(x => x.Id == id);
        }

        public List<Invoice> GetAll()
        {
            return _db.Invoices
                .AsNoTracking()
                .ToList();
        }

        public Invoice GetByAcessKey(string acessKey)
        {
            return _db.Invoices
                .Where(x => x.AccessKey == acessKey)
                .AsNoTracking()
                .Include(x => x.InvoiceItems)
                .Include(x => x.ExpeditionOrder)
                .FirstOrDefault();
        }

        public async Task<Invoice> AddAsync(Invoice invoice)
        {
            invoice.EntryDate = DateTime.Now;
            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();
            return invoice;
        }
        


        public Invoice Add(Invoice invoice)
        {
            invoice.EntryDate = DateTime.Now;
            _db.Invoices.Add(invoice);
            _db.SaveChanges();
            return invoice;
        }

        public List<Invoice> AddRange(List<Invoice> invoices)
        {
            foreach (Invoice invoice in invoices)
            {
                invoice.EntryDate = DateTime.Now;
            }

            _db.Invoices.AddRange(invoices);
            _db.SaveChanges();
            return invoices;
        }

        public Invoice Update(Invoice invoice)
        {
            Invoice invoiceById = GetById(invoice.Id);
            if (invoiceById == null)
                throw new System.Exception("Houve um erro na atualização da nota");

            invoiceById.Number = invoice.Number;
            invoiceById.SerialNumber = invoice.SerialNumber;
            invoiceById.AccessKey = invoice.AccessKey;
            invoiceById.EmitDocument = invoice.EmitDocument;
            invoiceById.DestDocument = invoice.DestDocument;
            invoiceById.Status = invoice.Status;
            invoiceById.ExpeditionOrderId = invoice.ExpeditionOrderId;

            _db.Invoices.Update(invoiceById);
            _db.SaveChanges();
            return invoiceById;
        }

        public bool Delete(int id)
        {
            Invoice invoiceDb = GetById(id);
            if (invoiceDb == null) return false;

            _db.Invoices.Remove(invoiceDb);
            _db.SaveChanges();
            return true;
        }


        public async Task<Invoice> GetByAcessKeyAsync(string? invoiceAccessKey)
        {
            return await _db.Invoices.Include(e => e.InvoiceItems)
                .Where(x => x.AccessKey == invoiceAccessKey)
                .FirstOrDefaultAsync();
        }

        public async Task<Invoice> UpdateAsync(Invoice invoice)
        {
            _db.Invoices.Update(invoice);
            await _db.SaveChangesAsync();
            return invoice;
        }
    }
}
