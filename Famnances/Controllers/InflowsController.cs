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

        public InflowsController(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        public async Task<IActionResult> Index()
        {

            var inflow = await _httpHelper.Get<List<Inflow>>($"{Constants.INFLOWS_URI}");
            return View(inflow);
        }

        // GET: Inflows/Create
        public IActionResult Create()
        {
            //ViewData["UserId"] = new SelectList(_context.User, "Id", "Address");
            return View();
        }

        // POST: Inflows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Value,DateTimeStamp")] Inflow inflow)
        {
            if (ModelState.IsValid)
            {
                inflow.Id = Guid.NewGuid();
                inflow = await _httpHelper.Post<Inflow>($"{Constants.INFLOWS_URI}", inflow);
                return RedirectToAction(nameof(Index));
            }
            //ViewData["UserId"] = new SelectList(_context.User, "Id", "Address", inflow.UserId);
            return View(inflow);
        }

        // GET: Inflows/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inflow = await _httpHelper.Get<Inflow>($"{Constants.INFLOWS_URI}/{id}");
            if (inflow == null)
            {
                return NotFound();
            }
            //ViewData["UserId"] = new SelectList(_context.User, "Id", "Address", inflow.UserId);
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
                    inflow = await _httpHelper.Put<Inflow>($"{Constants.INFLOWS_URI}/{id}", inflow);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _httpHelper.Get<Inflow>($"{Constants.INFLOWS_URI}/{id}") == null)
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
            //ViewData["UserId"] = new SelectList(_context.User, "Id", "Address", inflow.UserId);
            return View(inflow);
        }

        // POST: Inflows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var inflow = await _httpHelper.Get<Inflow>($"{Constants.INFLOWS_URI}/{id}");
            if (inflow != null)
            {
                await _httpHelper.Delete($"{Constants.INFLOWS_URI}/{id}");
            }
            return RedirectToAction(nameof(Index));
        }


        #region FixedIncome

        [HttpGet]
        public async Task<IActionResult> IndexFixedIncomes()
        {
            var fixedIncomes = await _httpHelper.Get<List<FixedIncome>>($"{Constants.FIXED_INCOMES_URI}");
            return View(fixedIncomes);
        }

        [HttpGet]
        public async Task<IActionResult> CreateFixedIncomes()
        {
            ViewBag.Periods = new SelectList(await _httpHelper.Get<List<Country>>($"{Constants.PERIODS_URI}"), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFixedIncomes(FixedIncome entity)
        {
            entity = await _httpHelper.Post<FixedIncome>($"{Constants.FIXED_INCOMES_URI}", entity);
            return RedirectToAction("FixedIncomeIndex");
        }

        [HttpGet]
        public async Task<IActionResult> EditFixedIncomes(Guid id)
        {
            var fixedIncome = await _httpHelper.Get<FixedIncome>($"{Constants.FIXED_INCOMES_URI}/{id}");
            ViewBag.Periods = new SelectList(await _httpHelper.Get<List<Country>>($"{Constants.PERIODS_URI}"), "Id", "Name", fixedIncome.PayablePeriodId);
            return View(fixedIncome);
        }

        [HttpPost]
        public async Task<IActionResult> EditFixedIncomes(FixedIncome entity)
        {
            entity = await _httpHelper.Put<FixedIncome>($"{Constants.FIXED_INCOMES_URI}", entity);
            return RedirectToAction("FixedIncomeIndex");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFixedIncome(Guid id)
        {
            await _httpHelper.Delete<FixedIncome>($"{Constants.FIXED_INCOMES_URI}/{id}");
            return RedirectToAction("FixedIncomeIndex");
        }

        #endregion
    }
}
