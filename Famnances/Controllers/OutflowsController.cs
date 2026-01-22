using Famnances.Core.Security.Authorization;
using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Entities;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Famnances.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
    public class OutflowsController : Controller
    {
        IHttpHelper _httpHelper;
        ILanguageHelper _utilities;

        public OutflowsController(IHttpHelper httpHelper, ILanguageHelper utilities)
        {
            _httpHelper = httpHelper;
            _utilities = utilities;
        }

        // GET: Outflows

        [ServiceFilter(typeof(HeaderSummaryFilter))]
        public async Task<IActionResult> Index()
        {
            var from = HttpContext.Session.GetString(Constants.DATE_FROM);
            var to = HttpContext.Session.GetString(Constants.DATE_TO);
            var outflow = await _httpHelper.Get<List<Outflow>>($"{Constants.OUTFLOWS_URI}/{from}/{to}");
            return View(outflow);
        }

        // GET: Outflows/Create        
        public async Task<IActionResult> Create()
        {
            var budgets = await _httpHelper.Get<List<ExpensesBudget>>($"{Constants.BUDGETS_URI}");
            ViewData["ExpenseBudgetId"] = new SelectList(budgets, "Id", "Name");
            return View();
        }

        // POST: Outflows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Value,TransactionDate,ExpenseBudgetId")] Outflow outflow)
        {
            if (ModelState.IsValid)
            {
                outflow.Id = Guid.NewGuid();
                outflow = await _httpHelper.Post<Outflow>(Constants.OUTFLOWS_URI, outflow);
                return RedirectToAction(nameof(Index));
            }

            var budgets = await _httpHelper.Get<List<ExpensesBudget>>($"{Constants.BUDGETS_URI}");
            ViewData["ExpenseBudgetId"] = new SelectList(budgets, "Id", "Name", outflow.ExpenseBudgetId);
            return View(outflow);
        }

        // GET: Outflows/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var outflow = await _httpHelper.Get<Outflow>($"{Constants.OUTFLOWS_URI}/{id}");
            if (outflow == null)
            {
                return NotFound();
            }
            var budgets = await _httpHelper.Get<List<ExpensesBudget>>($"{Constants.BUDGETS_URI}");
            ViewData["ExpenseBudgetId"] = new SelectList(budgets, "Id", "Name", outflow.ExpenseBudgetId);
            return View(outflow);
        }

        // POST: Outflows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Description,Value,TransactionDate,ExpenseBudgetId")] Outflow outflow)
        {
            if (id != outflow.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _httpHelper.Put($"{Constants.OUTFLOWS_URI}/{id}", outflow);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await OutflowExists(outflow.Id))
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
            var budgets = await _httpHelper.Get<List<ExpensesBudget>>($"{Constants.BUDGETS_URI}");
            ViewData["ExpenseBudgetId"] = new SelectList(budgets, "Id", "Name", outflow.ExpenseBudgetId);
            return View(outflow);
        }

        // POST: Outflows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var outflow = await _httpHelper.Get<Outflow>($"{Constants.OUTFLOWS_URI}/{id}");
            if (outflow != null)
            {
                await _httpHelper.Delete($"{Constants.OUTFLOWS_URI}/{id}");
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> OutflowExists(Guid id)
        {
            return await _httpHelper.Get<Outflow>($"{Constants.OUTFLOWS_URI}/{id}") != null;
        }

        [ServiceFilter(typeof(HeaderSummaryFilter))]
        public async Task<IActionResult> IndexFixedExpenses()
        {
            var fixedExpense = await _httpHelper.Get<List<FixedExpense>>($"{Constants.FIXED_EXPENSES_URI}");
            return View(fixedExpense);
        }

        public async Task<IActionResult> DetailsFixedExpenses(Guid? id)
        {
            var from = HttpContext.Session.GetString(Constants.DATE_FROM);
            var to = HttpContext.Session.GetString(Constants.DATE_TO);
            var fixedExpense = await _httpHelper.Get<FixedExpense>($"{Constants.FIXED_EXPENSES_URI}/{id}/{from}/{to}");
            return View(fixedExpense);
        }

        // GET: FixedExpenses/Create
        public async Task<IActionResult> CreateFixedExpenses()
        {
            ViewBag.Periods = await _utilities.GetPeriodDropdown(Thread.CurrentThread.CurrentUICulture.ToString());
            return View();
        }

        // POST: FixedExpenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFixedExpenses([Bind("Id,Name,Value,StartDate,EndDate,Active,PeriodId,ShareOnHousehold")] FixedExpense fixedExpense)
        {
            if (ModelState.IsValid)
            {
                fixedExpense.Id = Guid.NewGuid();
                fixedExpense = await _httpHelper.Post<FixedExpense>($"{Constants.FIXED_EXPENSES_URI}", fixedExpense);
                return RedirectToAction(nameof(IndexFixedExpenses));
            }

            ViewBag.Periods = await _utilities.GetPeriodDropdown(Thread.CurrentThread.CurrentUICulture.ToString());
            return View(fixedExpense);
        }

        // GET: FixedExpenses/Edit/5
        public async Task<IActionResult> EditFixedExpenses(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fixedExpense = await _httpHelper.Get<FixedExpense>($"{Constants.FIXED_EXPENSES_URI}/{id}");
            if (fixedExpense == null)
            {
                return NotFound();
            }

            ViewBag.Periods = await _utilities.GetPeriodDropdown(Thread.CurrentThread.CurrentUICulture.ToString());
            return View(fixedExpense);
        }

        // POST: FixedExpenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFixedExpenses(Guid id, [Bind("Id,Name,Value,StartDate,EndDate,Active,PeriodId,ShareOnHousehold")] FixedExpense fixedExpense)
        {
            if (id != fixedExpense.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _httpHelper.Put($"{Constants.FIXED_EXPENSES_URI}/{id}", fixedExpense);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await FixedExpenseExists(fixedExpense.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(IndexFixedExpenses));
            }

            ViewBag.Periods = await _utilities.GetPeriodDropdown(Thread.CurrentThread.CurrentUICulture.ToString());
            return View(fixedExpense);
        }


        // POST: FixedExpenses/Delete/5
        [HttpPost, ActionName("DeleteFixedExpenses")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFixedExpenses(Guid id)
        {
            var fixedExpense = await _httpHelper.Get<FixedExpense>($"{Constants.FIXED_EXPENSES_URI}/{id}");
            if (fixedExpense != null)
            {
                await _httpHelper.Delete($"{Constants.FIXED_EXPENSES_URI}/{id}");
            }
            return RedirectToAction(nameof(IndexFixedExpenses));
        }

        private async Task<bool> FixedExpenseExists(Guid id)
        {
            return await _httpHelper.Get<FixedExpense>($"{Constants.FIXED_EXPENSES_URI}/{id}") != null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayFixedExpenses(Guid id)
        {
            await _httpHelper.Post($"{Constants.FIXED_EXPENSES_URI}/Pay?id={id}",null);
            return RedirectToAction("Index", "Home", new {date=DateTimeEast.Now.ToString("yyyy-MM-dd")});
        }
    }
}
