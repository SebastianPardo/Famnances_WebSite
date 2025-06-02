using Famnances.AuthMiddleware;
using Famnances.DataCore.Entities;
using Famnances.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
