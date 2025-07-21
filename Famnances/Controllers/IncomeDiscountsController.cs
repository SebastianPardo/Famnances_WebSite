using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Famnances.Controllers
{
    public class IncomeDiscountsController : Controller
    {
        DatabaseContext _context;
        public IncomeDiscountsController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.IncomeDiscount.Include(i => i.User);
            return View(await databaseContext.ToListAsync());
        }

        // GET: IncomeDiscounts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incomeDiscount = await _context.IncomeDiscount
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (incomeDiscount == null)
            {
                return NotFound();
            }

            return View(incomeDiscount);
        }

        // GET: IncomeDiscounts/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Address");
            return View();
        }

        // POST: IncomeDiscounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Value,IsPercentage,IsPrediscount,IsTax,Active,UserId")] IncomeDiscount incomeDiscount)
        {
            if (ModelState.IsValid)
            {
                incomeDiscount.Id = Guid.NewGuid();
                _context.Add(incomeDiscount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Address", incomeDiscount.UserId);
            return View(incomeDiscount);
        }

        // GET: IncomeDiscounts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incomeDiscount = await _context.IncomeDiscount.FindAsync(id);
            if (incomeDiscount == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Address", incomeDiscount.UserId);
            return View(incomeDiscount);
        }

        // POST: IncomeDiscounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Description,Value,IsPercentage,IsPrediscount,IsTax,Active,UserId")] IncomeDiscount incomeDiscount)
        {
            if (id != incomeDiscount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(incomeDiscount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IncomeDiscountExists(incomeDiscount.Id))
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
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Address", incomeDiscount.UserId);
            return View(incomeDiscount);
        }

        // GET: IncomeDiscounts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incomeDiscount = await _context.IncomeDiscount
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (incomeDiscount == null)
            {
                return NotFound();
            }

            return View(incomeDiscount);
        }

        // POST: IncomeDiscounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var incomeDiscount = await _context.IncomeDiscount.FindAsync(id);
            if (incomeDiscount != null)
            {
                _context.IncomeDiscount.Remove(incomeDiscount);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IncomeDiscountExists(Guid id)
        {
            return _context.IncomeDiscount.Any(e => e.Id == id);
        }
    }
}
