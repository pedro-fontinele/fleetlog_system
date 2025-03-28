using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LOGHouseSystem.Infra.Database;
using LOGHouseSystem.Models;
using LOGHouseSystem.Infra.Filters;

namespace LOGHouseSystem.Controllers.MVC
{
    [PageForLogedUser]
    public class AddressingStreetsController : Controller
    {
        private readonly AppDbContext _context;

        public AddressingStreetsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AddressingStreets
        public async Task<IActionResult> Index()
        {
            return _context.AddressingStreets != null ?
                        View(await _context.AddressingStreets.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.AddressingStreets'  is null.");
        }

        // GET: AddressingStreets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AddressingStreets == null)
            {
                return NotFound();
            }

            var addressingStreet = await _context.AddressingStreets
                .FirstOrDefaultAsync(m => m.AddressingStreetID == id);
            if (addressingStreet == null)
            {
                return NotFound();
            }

            return View(addressingStreet);
        }

        // GET: AddressingStreets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AddressingStreets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AddressingStreetID,Name,Status")] AddressingStreet addressingStreet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(addressingStreet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(addressingStreet);
        }

        // GET: AddressingStreets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AddressingStreets == null)
            {
                return NotFound();
            }

            var addressingStreet = await _context.AddressingStreets.FindAsync(id);
            if (addressingStreet == null)
            {
                return NotFound();
            }
            return View(addressingStreet);
        }

        // POST: AddressingStreets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AddressingStreetID,Name,Status")] AddressingStreet addressingStreet)
        {
            if (id != addressingStreet.AddressingStreetID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(addressingStreet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddressingStreetExists(addressingStreet.AddressingStreetID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(addressingStreet);
        }

        // GET: AddressingStreets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AddressingStreets == null)
            {
                return NotFound();
            }

            var addressingStreet = await _context.AddressingStreets
                .FirstOrDefaultAsync(m => m.AddressingStreetID == id);
            if (addressingStreet == null)
            {
                return NotFound();
            }

            return View(addressingStreet);
        }

        // POST: AddressingStreets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AddressingStreets == null)
            {
                return Problem("Entity set 'AppDbContext.AddressingStreets'  is null.");
            }
            var addressingStreet = await _context.AddressingStreets.FindAsync(id);
            if (addressingStreet != null)
            {
                _context.AddressingStreets.Remove(addressingStreet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AddressingStreetExists(int id)
        {
            return (_context.AddressingStreets?.Any(e => e.AddressingStreetID == id)).GetValueOrDefault();
        }
    }
}
