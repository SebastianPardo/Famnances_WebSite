using Famnances.Core.Security.Authorization;
using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Entities;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Famnances.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Famnances.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
    public class OutflowsController : Controller
    {
        IHttpHelper _httpHelper;
        ILanguageHelper _utilities;
        IStringLocalizer<PrettyError> _localizer;

        public OutflowsController(IHttpHelper httpHelper, ILanguageHelper utilities, IStringLocalizer<PrettyError> localizer)
        {
            _httpHelper = httpHelper;
            _utilities = utilities;
            _localizer = localizer;
        }

        #region Outflow

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
            return View(new OutflowViewModel());
        }

        // POST: Outflows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OutflowViewModel outflowViewModel)
        {
            var outflow = outflowViewModel.Outflow;

            TotalsByPeriod? totalsByPeriod = await _httpHelper.Get<TotalsByPeriod?>($"{Constants.TOTALSBYPERIOD_URI}/GetByDate/{outflow.TransactionDate.ToString("yyyy-MM-dd")}");
            if(totalsByPeriod == null)
            {
                TempData[Constants.ERROR] = _localizer[PrettyError.OUT_DATE];
                return View(outflowViewModel);
            }

            if (ModelState.IsValid)
            {
                var overspent = await ValidateOverSpent(outflow.ExpenseBudgetId, outflow.TransactionDate, outflow.Value);

                if (overspent == null)
                {
                    outflow.Id = Guid.NewGuid();
                    outflow = await _httpHelper.Post<Outflow>(Constants.OUTFLOWS_URI, outflow);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    outflowViewModel.OverSpent = overspent;
                    return View(outflowViewModel);
                }
            }

            var budgets = await _httpHelper.Get<List<ExpensesBudget>>($"{Constants.BUDGETS_URI}");
            ViewData["ExpenseBudgetId"] = new SelectList(budgets, "Id", "Name", outflow.ExpenseBudgetId);
            return View(outflowViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Overspent(OutflowViewModel outflowViewModel)
        {
            var outflow = outflowViewModel.Outflow;
            var overSpent = outflowViewModel.OverSpent;
            var budget = await _httpHelper.Get<ExpensesBudget>($"{Constants.BUDGETS_URI}/{outflow.ExpenseBudgetId}");

            if (!overSpent.IsFull)
            {
                outflow.Value = outflow.Value - overSpent.OverSpentValue;
                await _httpHelper.Post<Outflow>(Constants.OUTFLOWS_URI, outflow);
            }

            if (overSpent.IsSaving)
            {
                SavingRecord savingRecord = new SavingRecord
                {
                    IsEmergency = false,
                    IsExpense = true,
                    Description = $"Overspent from {budget.Name} - {outflow.Description}",
                    SavingsPocketId = overSpent.IdSelected,
                    Value = overSpent.IsFull? outflow.Value : overSpent.OverSpentValue,
                    TransactionDate = outflow.TransactionDate
                };
                await _httpHelper.Post<SavingRecord>(Constants.SAVINGS_URI, savingRecord);
            }
            else
            {
                Outflow outflowOverSpent = new Outflow
                {
                    Description = $"Overspent from {budget.Name} - {outflow.Description}",
                    ExpenseBudgetId = overSpent.IdSelected,
                    Value = overSpent.IsFull ? outflow.Value : overSpent.OverSpentValue,
                    TransactionDate = outflow.TransactionDate
                };
                await _httpHelper.Post<Outflow>(Constants.OUTFLOWS_URI, outflowOverSpent);
            }
            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(Guid id, OutflowViewModel outflowViewModel)
        {
            var outflow = outflowViewModel.Outflow;
            if (id != outflow.Id)
            {
                return NotFound();
            }

            var oldOutflow = await _httpHelper.Get<Outflow>($"{Constants.OUTFLOWS_URI}/{id}");

            var overSpent = await ValidateOverSpent(outflow.ExpenseBudgetId, outflow.TransactionDate, outflow.Value - oldOutflow.Value);
            if (ModelState.IsValid)
            {
                if (overSpent == null)
                {
                    try
                    {
                        await _httpHelper.Put($"{Constants.OUTFLOWS_URI}/{id}", outflow);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (oldOutflow == null)
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
                else
                {
                    outflowViewModel.OverSpent = overSpent;
                    return View(outflowViewModel);
                }
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

        private async Task<OverspentViewModel?> ValidateOverSpent(Guid budgetId, DateTime date, decimal value)
        {
            var budgetStatus = await _httpHelper
                .Get<ExpenseBudgetByPeriod>($"{Constants.BUDGETS_URI}/GetBalanceByIdDate/{budgetId}/{date.ToString("yyyy-MM-dd")}");
            var balace = budgetStatus.Budget - budgetStatus.Expense;

            if (balace < value && value > 0)
            {
                OverspentViewModel overspent = new OverspentViewModel
                {
                    FullValue = value,
                    OverSpentValue = -1 * (balace - value)
                };
                return overspent;
            }
            return null;
        }

        #endregion

        #region FixedExpenses

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
            await _httpHelper.Post($"{Constants.FIXED_EXPENSES_URI}/Pay?id={id}", null);
            return RedirectToAction("Index", "Home", new { date = DateTimeEast.Now.ToString("yyyy-MM-dd") });
        }
        #endregion
    }
}
