using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Famnances.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
    public class IncomeDiscountsController : Controller
    {
        IHttpHelper _httpHelper;
        public IncomeDiscountsController(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        public async Task<IActionResult> Index()
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var discounts = await _httpHelper.Get<List<IncomeDiscount>>($"{Constants.INCOME_DISCOUNTS_URI}");
            return View(discounts);
        }

        // GET: IncomeDiscounts/Create
        public IActionResult Create()
        {
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
                incomeDiscount = await _httpHelper.Post<IncomeDiscount>($"{Constants.INCOME_DISCOUNTS_URI}", incomeDiscount);
                return RedirectToAction(nameof(Index));
            }
            return View(incomeDiscount);
        }

        // GET: IncomeDiscounts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incomeDiscount = await _httpHelper.Get<IncomeDiscount>($"{Constants.INCOME_DISCOUNTS_URI}/{id}");
            if (incomeDiscount == null)
            {
                return NotFound();
            }
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
                    await _httpHelper.Put($"{Constants.INCOME_DISCOUNTS_URI}/{id}", incomeDiscount);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await IncomeDiscountExists(incomeDiscount.Id))
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
            return View(incomeDiscount);
        }


        // POST: IncomeDiscounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var discount = await _httpHelper.Get<ExpensesBudget>($"{Constants.INCOME_DISCOUNTS_URI}/{id}");
            if (discount != null)
            {
                await _httpHelper.Delete($"{Constants.INCOME_DISCOUNTS_URI}/{id}");
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> IncomeDiscountExists(Guid id)
        {
            return await _httpHelper.Get<IncomeDiscount>($"{Constants.INCOME_DISCOUNTS_URI}/{id}") != null;
        }
    }
}
