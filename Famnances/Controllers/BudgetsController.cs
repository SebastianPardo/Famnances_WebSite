using Famnances.DataCore.Entities;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Data;
namespace Famnances.Controllers
{
    public class BudgetsController : Controller
    {
        IHttpHelper _httpHelper;
        public BudgetsController(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }
        public async Task<IActionResult> Index()
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var budgets = await _httpHelper.Get<List<ExpensesBudget>>($"{Constants.BUDGETS_URI}/GetEditables");
            return View(budgets);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ExpensesBudget());
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,Value")]ExpensesBudget entity)
        {
            if (ModelState.IsValid)
            {
                var budgetType = await _httpHelper.Get<ExpensesBudgetType>($"{Constants.BUDGET_TYPES_URI}/GetByCode/PER");
                entity.Id = Guid.NewGuid();
                entity.BudgetTypeId = budgetType.Id;
                var budget = await _httpHelper.Post<ExpensesBudget>($"{Constants.BUDGETS_URI}", entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var budget = await _httpHelper.Get<ExpensesBudget>($"{Constants.BUDGETS_URI}/{id}");
            return View(budget);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Name,Value")] ExpensesBudget entity)
        {
            if (ModelState.IsValid)
            {
                await _httpHelper.Put($"{Constants.BUDGETS_URI}", entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var budget = await _httpHelper.Get<ExpensesBudget>($"{Constants.BUDGETS_URI}/{id}");
            if (budget != null)
            {
                await _httpHelper.Delete($"{Constants.BUDGETS_URI}/{id}");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
