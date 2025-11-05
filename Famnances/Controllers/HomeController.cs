using Famnances.Core.Security.Authorization;
using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
using Famnances.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Constants = Famnances.Helpers.Constants;

namespace Famnances.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IHttpHelper _httpHelper;

        public HomeController(ILogger<HomeController> logger, IHttpHelper httpHelper)
        {
            _logger = logger;
            _httpHelper = httpHelper;
        }

        public async Task<IActionResult> Index(DateTime date)
        {
            var totals = await _httpHelper.Get<TotalsByPeriod?>($"{Constants.TOTALSBYPERIOD_URI}/GetCurrentPeriod");
            if (totals == null)
            {
                totals = await _httpHelper.Get<TotalsByPeriod?>($"{Constants.ACCOUNTING_URI}/CalculatePeriod");
            }
            var homeSummary = await _httpHelper.Get<SummaryModel>($"{Constants.ACCOUNTING_URI}/CurentTotals/{date}");
            return View(homeSummary);
        }

        public async Task<IActionResult> PreviousPeriod(DateTime date)
        {
            date = date.AddDays(-1);
            var totals = await _httpHelper.Get<TotalsByPeriod?>($"{Constants.TOTALSBYPERIOD_URI}/GetByDate/{date}");
            if (totals == null)
                return RedirectToAction("Index", new { date = DateTimeEast.Now });
            else
                return RedirectToAction("Index", new { date = date });
        }

        public async Task<IActionResult> NextPeriod(DateTime date)
        {
            date = date.AddDays(1);
            var totals = await _httpHelper.Get<TotalsByPeriod?>($"{Constants.TOTALSBYPERIOD_URI}/GetByDate/{date}");
            if (totals == null)
                return RedirectToAction("Index", new { date = DateTimeEast.Now });
            else
                return RedirectToAction("Index", new { date = date });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
