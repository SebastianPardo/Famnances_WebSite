using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;

namespace Famnances.WebSite.Controllers
{
    public class SavingsController : Controller
    {
        private readonly DatabaseContext _context;

        public SavingsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: SavingRecords
        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.SavingRecord.Include(s => s.SavingsPocket);
            return View(await databaseContext.ToListAsync());
        }

        // GET: SavingRecords/Create
        public IActionResult Create()
        {
            ViewData["SavingsPocketId"] = new SelectList(_context.SavingsPocket, "Id", "Name");
            return View();
        }

        // POST: SavingRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,IsExpense,IsEmergency,TimeStamp,Value,SavingsPocketId")] SavingRecord savingRecord)
        {
            if (ModelState.IsValid)
            {
                savingRecord.Id = Guid.NewGuid();
                _context.Add(savingRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SavingsPocketId"] = new SelectList(_context.SavingsPocket, "Id", "Name", savingRecord.SavingsPocketId);
            return View(savingRecord);
        }

        // GET: SavingRecords/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savingRecord = await _context.SavingRecord.FindAsync(id);
            if (savingRecord == null)
            {
                return NotFound();
            }
            ViewData["SavingsPocketId"] = new SelectList(_context.SavingsPocket, "Id", "Name", savingRecord.SavingsPocketId);
            return View(savingRecord);
        }

        // POST: SavingRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Description,IsExpense,IsEmergency,TimeStamp,Value,SavingsPocketId")] SavingRecord savingRecord)
        {
            if (id != savingRecord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(savingRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SavingRecordExists(savingRecord.Id))
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
            ViewData["SavingsPocketId"] = new SelectList(_context.SavingsPocket, "Id", "Name", savingRecord.SavingsPocketId);
            return View(savingRecord);
        }

        // POST: SavingRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var savingRecord = await _context.SavingRecord.FindAsync(id);
            if (savingRecord != null)
            {
                _context.SavingRecord.Remove(savingRecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SavingRecordExists(Guid id)
        {
            return _context.SavingRecord.Any(e => e.Id == id);
        }

        public async Task<IActionResult> IndexPockets()
        {
            var databaseContext = _context.SavingsPocket.Include(s => s.User);
            return View(await databaseContext.ToListAsync());
        }

        // GET: SavingsPockets/Create
        public IActionResult CreatePockets()
        {
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Address");
            return View();
        }

        // POST: SavingsPockets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePockets([Bind("Id,Name,IsActive,ChallengeValue,Total,UserId")] SavingsPocket savingsPocket)
        {
            if (ModelState.IsValid)
            {
                savingsPocket.Id = Guid.NewGuid();
                _context.Add(savingsPocket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Address", savingsPocket.UserId);
            return View(savingsPocket);
        }

        // GET: SavingsPockets/Edit/5
        public async Task<IActionResult> EditPockets(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savingsPocket = await _context.SavingsPocket.FindAsync(id);
            if (savingsPocket == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Address", savingsPocket.UserId);
            return View(savingsPocket);
        }

        // POST: SavingsPockets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPockets(Guid id, [Bind("Id,Name,IsActive,ChallengeValue,Total,UserId")] SavingsPocket savingsPocket)
        {
            if (id != savingsPocket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(savingsPocket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SavingsPocketExists(savingsPocket.Id))
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
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Address", savingsPocket.UserId);
            return View(savingsPocket);
        }


        // POST: SavingsPockets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePockets(Guid id)
        {
            var savingsPocket = await _context.SavingsPocket.FindAsync(id);
            if (savingsPocket != null)
            {
                _context.SavingsPocket.Remove(savingsPocket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SavingsPocketExists(Guid id)
        {
            return _context.SavingsPocket.Any(e => e.Id == id);
        }
    }
}
