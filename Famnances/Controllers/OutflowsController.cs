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
    public class OutflowsController : Controller
    {
        private readonly DatabaseContext _context;

        public OutflowsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Outflows
        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.Outflow.Include(o => o.ExpensesBudget);
            return View(await databaseContext.ToListAsync());
        }

        // GET: Outflows/Create
        public IActionResult Create()
        {
            ViewData["ExpenseBudgetId"] = new SelectList(_context.ExpensesBudget, "Id", "Name");
            return View();
        }

        // POST: Outflows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Value,DateTimeStamp,ExpenseBudgetId")] Outflow outflow)
        {
            if (ModelState.IsValid)
            {
                outflow.Id = Guid.NewGuid();
                _context.Add(outflow);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExpenseBudgetId"] = new SelectList(_context.ExpensesBudget, "Id", "Name", outflow.ExpenseBudgetId);
            return View(outflow);
        }

        // GET: Outflows/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var outflow = await _context.Outflow.FindAsync(id);
            if (outflow == null)
            {
                return NotFound();
            }
            ViewData["ExpenseBudgetId"] = new SelectList(_context.ExpensesBudget, "Id", "Name", outflow.ExpenseBudgetId);
            return View(outflow);
        }

        // POST: Outflows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Description,Value,DateTimeStamp,ExpenseBudgetId")] Outflow outflow)
        {
            if (id != outflow.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(outflow);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OutflowExists(outflow.Id))
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
            ViewData["ExpenseBudgetId"] = new SelectList(_context.ExpensesBudget, "Id", "Name", outflow.ExpenseBudgetId);
            return View(outflow);
        }

        // POST: Outflows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var outflow = await _context.Outflow.FindAsync(id);
            if (outflow != null)
            {
                _context.Outflow.Remove(outflow);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OutflowExists(Guid id)
        {
            return _context.Outflow.Any(e => e.Id == id);
        }

        public async Task<IActionResult> IndexFixedExpenses()
        {
            var databaseContext = _context.FixedExpense.Include(f => f.Period).Include(f => f.User);
            return View(await databaseContext.ToListAsync());
        }

        // GET: FixedExpenses/Create
        public IActionResult CreateFixedExpenses()
        {
            ViewData["PeriodId"] = new SelectList(_context.Period, "Id", "Code");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Address");
            return View();
        }

        // POST: FixedExpenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFixedExpenses([Bind("Id,Name,Value,StartDate,EndDate,Active,PeriodId,UserId")] FixedExpense fixedExpense)
        {
            if (ModelState.IsValid)
            {
                fixedExpense.Id = Guid.NewGuid();
                _context.Add(fixedExpense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PeriodId"] = new SelectList(_context.Period, "Id", "Code", fixedExpense.PeriodId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Address", fixedExpense.UserId);
            return View(fixedExpense);
        }

        // GET: FixedExpenses/Edit/5
        public async Task<IActionResult> EditFixedExpenses(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fixedExpense = await _context.FixedExpense.FindAsync(id);
            if (fixedExpense == null)
            {
                return NotFound();
            }
            ViewData["PeriodId"] = new SelectList(_context.Period, "Id", "Code", fixedExpense.PeriodId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Address", fixedExpense.UserId);
            return View(fixedExpense);
        }

        // POST: FixedExpenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFixedExpenses(Guid id, [Bind("Id,Name,Value,StartDate,EndDate,Active,PeriodId,UserId")] FixedExpense fixedExpense)
        {
            if (id != fixedExpense.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fixedExpense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FixedExpenseExists(fixedExpense.Id))
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
            ViewData["PeriodId"] = new SelectList(_context.Period, "Id", "Code", fixedExpense.PeriodId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Address", fixedExpense.UserId);
            return View(fixedExpense);
        }


        // POST: FixedExpenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFixedExpenses(Guid id)
        {
            var fixedExpense = await _context.FixedExpense.FindAsync(id);
            if (fixedExpense != null)
            {
                _context.FixedExpense.Remove(fixedExpense);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FixedExpenseExists(Guid id)
        {
            return _context.FixedExpense.Any(e => e.Id == id);
        }
    }
}
