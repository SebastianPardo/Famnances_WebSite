using Famnances.DataCore.Entities;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Data;
using System.Threading.Tasks;

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
            var budgets = await _httpHelper.Get<List<ExpensesBudget>>($"{Constants.BUDGETS_URI}");
            return View(budgets);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ExpensesBudget());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExpensesBudget entity)
        {
            if (ModelState.IsValid)
            {
                entity.Id = Guid.NewGuid();
                var budget = await _httpHelper.Post<List<ExpensesBudget>>($"{Constants.BUDGETS_URI}", entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        public IActionResult Edit(Guid id)
        {
            return View();
        }

        public IActionResult Edit(ExpensesBudget entity)
        {
            return View(entity);
        }

        public IActionResult Delete(Guid id)
        {
            return View();
        }
    }
}
