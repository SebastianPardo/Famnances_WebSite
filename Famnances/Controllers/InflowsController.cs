using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
using Famnances.Helpers.Interfaces;
using Famnances.Models.ViewModels;
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
        public async Task<IActionResult> Create()
        {
            var discounts = await _httpHelper.Get<List<IncomeDiscount>>($"{Constants.INCOME_DISCOUNTS_URI}");
            ViewData["Discounts"] = new SelectList(discounts, "Id", "Description");
            return View();
        }

        // POST: Inflows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IncomeTransactionModel inflow)
        {
            if (ModelState.IsValid)
            {
                inflow.Income.Id = Guid.NewGuid();
                inflow = await _httpHelper.Post<IncomeTransactionModel>($"{Constants.INFLOWS_URI}", inflow);
                return RedirectToAction(nameof(Index));
            }
            var discounts = await _httpHelper.Get<List<IncomeDiscount>>($"{Constants.INCOME_DISCOUNTS_URI}");
            ViewData["Discounts"] = new SelectList(discounts, "Id", "Address");
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
        public async Task<IActionResult> Edit(Guid id, IncomeTransactionModel inflow)
        {
            if (id != inflow.Income.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    inflow.Income = await _httpHelper.Put<Inflow>($"{Constants.INFLOWS_URI}/{id}", inflow.Income);
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
            ViewBag.Discounts = new SelectList(await _httpHelper.Get<List<IncomeDiscount>>($"{Constants.INCOME_DISCOUNTS_URI}"), "Id", "Description");
            ViewBag.Periods = new SelectList(await _httpHelper.Get<List<Country>>($"{Constants.PERIODS_URI}"), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFixedIncomes(FixedIncomeViewModel entity)
        {
            entity.FixedIncome.FixedIncomeByDiscount = entity.SelectedIncomeDiscountIds.Select(e=> new FixedIncomeByDiscount { FixedIncomeId = e}).ToList();
            entity.FixedIncome = await _httpHelper.Post<FixedIncome>($"{Constants.FIXED_INCOMES_URI}", entity.FixedIncome);
            return RedirectToAction("IndexFixedIncomes");
        }

        [HttpGet]
        public async Task<IActionResult> EditFixedIncomes(Guid id)
        {
            var fixedIncome = await _httpHelper.Get<FixedIncome>($"{Constants.FIXED_INCOMES_URI}/{id}");
            ViewBag.Periods = new SelectList(await _httpHelper.Get<List<Country>>($"{Constants.PERIODS_URI}"), "Id", "Name", fixedIncome.PayablePeriodId);
            return View(fixedIncome);
        }

        [HttpPost]
        public async Task<IActionResult> EditFixedIncomes(FixedIncomeViewModel entity)
        {
            entity.FixedIncome.FixedIncomeByDiscount = entity.SelectedIncomeDiscountIds.Select(e => new FixedIncomeByDiscount { FixedIncomeId = e }).ToList();
            entity.FixedIncome = await _httpHelper.Put<FixedIncome>($"{Constants.FIXED_INCOMES_URI}", entity.FixedIncome);
            return RedirectToAction("IndexFixedIncomes");
        }

        [HttpPost, ActionName("DeleteFixedIncome")]
        public async Task<IActionResult> DeleteFixedIncome(Guid id)
        {
            await _httpHelper.Delete<FixedIncome>($"{Constants.FIXED_INCOMES_URI}/{id}");
            return RedirectToAction("IndexFixedIncomes");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceiveFixedIncome(Guid id)
        {
            await _httpHelper.Post($"{Constants.FIXED_INCOMES_URI}/Receive?id={id}", null);
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}
