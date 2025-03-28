
using LOGHouseSystem.Infra.Helpers;
﻿using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Models;
using LOGHouseSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using NLog;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using LOGHouseSystem.Infra.Enums;
using LOGHouseSystem.Controllers.API.BarcodeColectorApi;
using LOGHouseSystem.Repositories.Interfaces;
using PagedList;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace LOGHouseSystem.Repositories
{
    public class ReceiptNoteRepository : RepositoryBase, IReceiptNoteRepository
    {
        private SessionHelper _session = new SessionHelper();

        public ReceiptNote GetById(int id)
        {
            return _db.ReceiptNotes
                .Include(x => x.ReceiptNoteItems)
                .Include(x => x.Client)
                .FirstOrDefault(x => x.Id == id);
        }

        public List<ReceiptNote> GetAll()
        {
            return _db.ReceiptNotes
                .AsNoTracking()
                .ToList();
        }

        public ReceiptNote GetByAcessKey(string acessKey)
        {
            return _db.ReceiptNotes
                .Where(x => x.AccessKey == acessKey)
                .AsNoTracking()
                .Include(x => x.ReceiptNoteItems)
                .Include(x => x.Client)
                .FirstOrDefault();
        }

        public async Task<ReceiptNote> AddAsync(ReceiptNote note)
        {
            note.EntryDate = DateTime.Now;
            _db.ReceiptNotes.Add(note);
            await _db.SaveChangesAsync();
            return note;
        }

        public List<ReceiptNote> GetByEntryDate(DateTime date)
        {
            return _db.ReceiptNotes
                .Where(x => x.EntryDate >= date && x.EntryDate < (date.AddDays(1)))
                .Include(x => x.Client)
                .AsNoTracking()
                .ToList();
        }

        public List<ReceiptNote> GetByStatus(NoteStatus noteStatus)
        {
            return _db.ReceiptNotes
                .Where(x => x.Status == noteStatus)
                .AsNoTracking()
                .Include(x => x.ReceiptNoteItems)
                .Include(x => x.Client)
                .ToList();
        }

        public List<ReceiptNote> GetDevolutionsNote()
        {
            return _db.ReceiptNotes
                .Where(x => x.IsDevolution == YesOrNo.Yes)
                .AsNoTracking()
                .Include(x => x.ReceiptNoteItems)
                .Include(x => x.Client)
                .ToList();
        }

        public List<ReceiptNote> GetByClient()
        {
            User userLoged = _session.SearchUserSession();

            if (userLoged == null || userLoged.Id <= 0)
                throw new Exception("Erro ao buscar usuário");
            

            Client client = _db.Clients.FirstOrDefault(x => x.UserId == userLoged.Id);

            if (client == null)
                throw new Exception("Erro ao buscar cliente");

            return _db.ReceiptNotes
                .Where(x => x.ClientId == client.Id)
                .AsNoTracking()
                .ToList();

        }


        public ReceiptNote Add(ReceiptNote note)
        {
            note.EntryDate = DateTime.Now;
            _db.ReceiptNotes.Add(note);
            _db.SaveChanges();
            return note;
        }

        public List<ReceiptNote> AddRange(List<ReceiptNote> notes)
        {
            foreach (ReceiptNote note in notes)
            {
                note.EntryDate = DateTime.Now;
            }
     
            _db.ReceiptNotes.AddRange(notes);
            _db.SaveChanges();
            return notes;
        }

        public ReceiptNote Update(ReceiptNote note)
        {
            ReceiptNote noteById = GetById(note.Id);
            if (noteById == null)
                throw new System.Exception("Houve um erro na atualização da nota");

            noteById.Number = note.Number;
            noteById.SerialNumber = note.SerialNumber;
            noteById.AccessKey = note.AccessKey;
            noteById.EmitDocument = note.EmitDocument;
            noteById.DestDocument = note.DestDocument;
            noteById.Status = note.Status;
            //noteById.ClientId = note.ClientId;
            //noteById.Client = note.Client;
            //noteById.ReceiptNoteItems = note.ReceiptNoteItems;

            _db.ReceiptNotes.Update(noteById);
            _db.SaveChanges();
            return noteById;
        }

        public bool Delete(int id)
        {
            ReceiptNote noteDb = GetById(id);
            if (noteDb == null) return false;

            _db.ReceiptNotes.Remove(noteDb);
            _db.SaveChanges();
            return true;
        }

        public List<ReceiptNote> GetByFilter(BarcodeColectorFilter filter)
        {
            var query = _db.ReceiptNotes.Select(x=>x);

            if (filter.ClientId != null)
                query = query.Where(x => x.ClientId == filter.ClientId);

            if (filter.ReceiptDate != null)
                query = query.Where(x => x.EntryDate >= filter.ReceiptDate);

            if (filter.NoteStatus != null)
                query = query.Where(x => x.Status == filter.NoteStatus);

            if (filter.Id != null)
                query = query.Where(x => x.Id == filter.Id);


            return query
                .AsNoTracking()
                .ToList();
        }

        public PagedList<ReceiptNote> GetByFilters(FilterViewModel filter)
        {
            int PageSize = 50;            

            var query = GetQueryableFilter(filter);

            return (PagedList<ReceiptNote>)query
                .AsNoTracking()
                //.Include(x => x.ReceiptNoteItems)
                .ToPagedList(filter.Page, PageSize);
        }

        public IQueryable<ReceiptNote> GetQueryableFilter(FilterViewModel filter)
        {           
            var query = _db.ReceiptNotes
                .Include(x => x.Client)
                .Include(x => x.ReceiptNoteItems)
                .AsQueryable();

            if (filter.ClientId != null)
                query = query.Where(x => x.ClientId == filter.ClientId);

            if (filter.EntryDateStart != null && filter.EntryDateStart != new DateTime(1,1, 1, 0, 0, 0))
                query = query.Where(x => x.EntryDate >= new DateTime(filter.EntryDateStart.Value.Year, filter.EntryDateStart.Value.Month, filter.EntryDateStart.Value.Day, 0, 0, 0));

            if (filter.EntryDateEnd != null && filter.EntryDateEnd != new DateTime(1, 1, 1, 0, 0, 0))
                query = query.Where(x => x.EntryDate <= new DateTime(filter.EntryDateEnd.Value.Year, filter.EntryDateEnd.Value.Month, filter.EntryDateEnd.Value.Day, 23, 59, 59));

            if (filter.NoteStatus != null)
                query = query.Where(x => x.Status == filter.NoteStatus);

            if (filter.InvoiceNumber != null)
                query = query.Where(x => x.Number == filter.InvoiceNumber);

            if (filter.IssueDate != null)
                query = query.Where(x => x.IssueDate >= new DateTime(filter.IssueDate.Value.Year, filter.IssueDate.Value.Month, filter.IssueDate.Value.Day, 0, 0, 0) && x.IssueDate <= new DateTime(filter.IssueDate.Value.Year, filter.IssueDate.Value.Month, filter.IssueDate.Value.Day, 23, 59, 59));

            query = query.OrderByDescending(x => x.Id);

            return query;
        }

        public async Task<ReceiptNote> GetByAcessKeyAsync(string? invoiceAccessKey)
        {
            return await _db.ReceiptNotes.Include(e => e.ReceiptNoteItems)
                .Where(x => x.AccessKey == invoiceAccessKey)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ReceiptNote>> GetAllById(IEnumerable<int> receiptNotesIds)
        {
            return await _db.ReceiptNotes
                .Include(x => x.ReceiptNoteItems)                
                .Include(x => x.Client)
                .Where(x => receiptNotesIds.Any(a => a == x.Id))
                .ToListAsync();
        }

        public async Task<ReceiptNote> GetByIdAsync(int receiptNoteId)
        {
            return await _db.ReceiptNotes
                .Include(x => x.ReceiptNoteItems)
                .Include(x => x.Client)
                .FirstOrDefaultAsync(x => x.Id == receiptNoteId);
        }

        public async Task<ReceiptNote> GetLastReceiptNoteAsync(int? clientId)
        {
            return await _db.ReceiptNotes
                .Include(x => x.ReceiptNoteItems)
                .Include(x => x.Client)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(x => x.ClientId == clientId);
        }
    }
}
