using Famnances.Core.Errors;
using Famnances.Core.Security.Authorization;
using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
using Famnances.Helpers.Interfaces;
using Famnances.Models.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Constants = Famnances.Helpers.Constants;

namespace Famnances.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IHttpHelper _httpHelper;

        public HomeController(ILogger<HomeController> logger, IHttpHelper httpHelper)
        {
            _logger = logger;
            _httpHelper = httpHelper;
        }

        public async Task<IActionResult> Index()
        {
            TotalsByPeriod? totals;

            if (string.IsNullOrEmpty(HttpContext.Session.GetString(Constants.DATE_FROM)))
                totals = await _httpHelper.Get<TotalsByPeriod?>($"{Constants.TOTALSBYPERIOD_URI}/GetCurrentPeriod");
            else
                totals = await _httpHelper.Get<TotalsByPeriod?>($"{Constants.TOTALSBYPERIOD_URI}/GetByDate/{HttpContext.Session.GetString(Constants.DATE_FROM)}");

            if (totals == null)
                totals = await _httpHelper.Get<TotalsByPeriod?>($"{Constants.ACCOUNTING_URI}/CalculatePeriod");

            HttpContext.Session.SetString(Constants.DATE_FROM, totals.PeriodDateStart.ToString("yyyy-MM-dd"));
            HttpContext.Session.SetString(Constants.DATE_TO, totals.PeriodDateEnd.ToString("yyyy-MM-dd"));
            var homeSummary = await _httpHelper.Get<SummaryModel>($"{Constants.ACCOUNTING_URI}/CurentTotals/{totals.PeriodDateStart.AddDays(1)}");
            return View(homeSummary);
        }

        public async Task<IActionResult> PreviousPeriod()
        {
            var date = DateTime.Parse(HttpContext.Session.GetString(Constants.DATE_FROM)).AddDays(-1).ToString("yyyy-MM-dd");

            var totals = await _httpHelper.Get<TotalsByPeriod?>($"{Constants.TOTALSBYPERIOD_URI}/GetByDate/{date}");
            if (totals != null)
            {
                HttpContext.Session.SetString(Constants.DATE_FROM, totals.PeriodDateStart.ToString("yyyy-MM-dd"));
                HttpContext.Session.SetString(Constants.DATE_TO, totals.PeriodDateEnd.ToString("yyyy-MM-dd"));
            }
            else
            {
                HttpContext.Session.Remove(Constants.DATE_FROM);
                HttpContext.Session.Remove(Constants.DATE_TO);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CurrentPeriod()
        {
            HttpContext.Session.Remove(Constants.DATE_FROM);
            HttpContext.Session.Remove(Constants.DATE_TO);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> NextPeriod()
        {
            var date = DateTime.Parse(HttpContext.Session.GetString(Constants.DATE_TO)).AddDays(1).ToString("yyyy-MM-dd");
            var totals = await _httpHelper.Get<TotalsByPeriod?>($"{Constants.TOTALSBYPERIOD_URI}/GetByDate/{date}");
            if (totals != null)
            {
                HttpContext.Session.SetString(Constants.DATE_FROM, totals.PeriodDateStart.ToString("yyyy-MM-dd"));
                HttpContext.Session.SetString(Constants.DATE_TO, totals.PeriodDateEnd.ToString("yyyy-MM-dd"));
            }
            else
            {
                HttpContext.Session.Remove(Constants.DATE_FROM);
                HttpContext.Session.Remove(Constants.DATE_TO);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (feature?.Error == null)
                return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });

            Exception ex = feature.Error;

            // Determine final status code
            int status = ex switch
            {
                AppException => 3312,
                UnauthorizedAccessException => 401,
                KeyNotFoundException => 404,
                ArgumentException => 400,
                _ => 500
            };

            ViewBag.Message = ex is AppException appEx
                ? appEx.Message
                : "Unexpected error occurred.";

            var log = new ErrorLog
            {
                StatusCode = status,
                Timestamp = DateTimeEast.Now,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Path = feature.Path,
                HttpMethod = HttpContext.Request.Method,
                QueryString = HttpContext.Request.QueryString.ToString()
            };

            var saved = await _httpHelper.Post<ErrorLog?>(Constants.ERROR_LOG_URI, log);

            return View(new ErrorViewModel
            {
                RequestId = saved?.Id.ToString() ??
                            Activity.Current?.Id ??
                            HttpContext.TraceIdentifier
            });
        }

    }
}
