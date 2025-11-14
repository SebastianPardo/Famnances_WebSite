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
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;
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

        public async Task<IActionResult> Index(DateTime date)
        {
            var totals = await _httpHelper.Get<TotalsByPeriod?>($"{Constants.TOTALSBYPERIOD_URI}/GetCurrentPeriod");
            if (totals == null)
            {
                totals = await _httpHelper.Get<TotalsByPeriod?>($"{Constants.ACCOUNTING_URI}/CalculatePeriod");
            }
            var homeSummary = await _httpHelper.Get<SummaryModel>($"{Constants.ACCOUNTING_URI}/CurentTotals/{date.ToString("yyyy-MM-dd")}");
            return View(homeSummary);
        }

        public async Task<IActionResult> PreviousPeriod(DateTime date)
        {
            date = date.AddDays(-1);
            var totals = await _httpHelper.Get<TotalsByPeriod?>($"{Constants.TOTALSBYPERIOD_URI}/GetByDate/{date.ToString("yyyy-MM-dd")}");
            if (totals == null)
                return RedirectToAction("Index", new { date = DateTimeEast.Now });
            else
                return RedirectToAction("Index", new { date = date });
        }

        public async Task<IActionResult> NextPeriod(DateTime date)
        {
            date = date.AddDays(1);
            var totals = await _httpHelper.Get<TotalsByPeriod?>($"{Constants.TOTALSBYPERIOD_URI}/GetByDate/{date.ToString("yyyy-MM-dd")}");
            if (totals == null)
                return RedirectToAction("Index", new { date = DateTimeEast.Now });
            else
                return RedirectToAction("Index", new { date = date });
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
