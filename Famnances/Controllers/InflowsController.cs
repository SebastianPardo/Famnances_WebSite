using Famnances.AuthMiddleware;
using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using Famnances.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Constants = Famnances.Helpers.Constants;

namespace Famnances.Controllers
{
    //[Authorize]
    public class InflowsController : Controller
    {
        IHttpHelper _httpHelper;
        private readonly DatabaseContext _context;

        public InflowsController(IHttpHelper httpHelper, DatabaseContext context)
        {
            _httpHelper = httpHelper;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.Inflow.Include(i => i.User);
            return View(await databaseContext.ToListAsync());
        }

        // GET: Inflows/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inflow = await _context.Inflow
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inflow == null)
            {
                return NotFound();
            }

            return View(inflow);
        }

        // GET: Inflows/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Address");
            return View();
        }

        // POST: Inflows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Value,DateTimeStamp,UserId")] Inflow inflow)
        {
            if (ModelState.IsValid)
            {
                inflow.Id = Guid.NewGuid();
                _context.Add(inflow);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Address", inflow.UserId);
            return View(inflow);
        }

        // GET: Inflows/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inflow = await _context.Inflow.FindAsync(id);
            if (inflow == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Address", inflow.UserId);
            return View(inflow);
        }

        // POST: Inflows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Description,Value,DateTimeStamp,UserId")] Inflow inflow)
        {
            if (id != inflow.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inflow);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InflowExists(inflow.Id))
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
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Address", inflow.UserId);
            return View(inflow);
        }

        // GET: Inflows/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inflow = await _context.Inflow
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inflow == null)
            {
                return NotFound();
            }

            return View(inflow);
        }

        // POST: Inflows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var inflow = await _context.Inflow.FindAsync(id);
            if (inflow != null)
            {
                _context.Inflow.Remove(inflow);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InflowExists(Guid id)
        {
            return _context.Inflow.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> InflowIndex()
        {
            var fixedIncomes = await _httpHelper.Get<List<FixedIncome>>($"{Constants.INFLOWS_URI}/GetFixedIncomes");
            return View();
        }

        #region FixedIncome

        [HttpGet]
        public async Task<IActionResult> FixedIncomeIndex()
        {
            var fixedIncomes = await _httpHelper.Get<List<FixedIncome>>($"{Constants.FIXED_INCOMES_URI}");
            return View(fixedIncomes);
        }

        [HttpGet]
        public async Task<IActionResult> FixedIncomeCreate()
        {
            ViewBag.Periods = new SelectList(await _httpHelper.Get<List<Country>>($"{Constants.PERIODS_URI}"), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FixedIncomeCreate(FixedIncome entity)
        {
            entity = await _httpHelper.Post<FixedIncome>($"{Constants.FIXED_INCOMES_URI}", entity);
            return RedirectToAction("FixedIncomeIndex");
        }

        [HttpGet]
        public async Task<IActionResult> FixedIncomeDetails(Guid id)
        {
            var fixedIncome = await _httpHelper.Get<FixedIncome>($"{Constants.FIXED_INCOMES_URI}/{id}");
            ViewBag.Periods = new SelectList(await _httpHelper.Get<List<Country>>($"{Constants.PERIODS_URI}"), "Id", "Name", fixedIncome.PayablePeriodId);
            return View(fixedIncome);
        }

        [HttpPost]
        public async Task<IActionResult> FixedIncomeUpdate(FixedIncome entity)
        {
            entity = await _httpHelper.Put<FixedIncome>($"{Constants.FIXED_INCOMES_URI}", entity);
            return RedirectToAction("FixedIncomeIndex");
        }

        [HttpPost]
        public async Task<IActionResult> FixedIncomeDelete(Guid id)
        {
            await _httpHelper.Delete<FixedIncome>($"{Constants.FIXED_INCOMES_URI}/{id}");
            return RedirectToAction("FixedIncomeIndex");
        }

        #endregion
    }
}
