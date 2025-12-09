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

        public async Task<IActionResult> Index()
        {
            var dateFrom = HttpContext.Session.GetString(Constants.DATE_FROM);
            MiniSummaryModel? miniSummaryModel = new MiniSummaryModel();

            if (string.IsNullOrEmpty(HttpContext.Session.GetString(Constants.DATE_FROM)))
                miniSummaryModel = await GetHeaderSummary(DateTimeEast.Now.ToString("yyyy-MM-dd"));
            else
                miniSummaryModel = await GetHeaderSummary(dateFrom);

            if (miniSummaryModel == null)
                await _httpHelper.Get<TotalsByPeriod?>($"{Constants.ACCOUNTING_URI}/CalculatePeriod");

            dateFrom = HttpContext.Session.GetString(Constants.DATE_FROM);
            var homeSummary = await _httpHelper.Get<SummaryModel>($"{Constants.ACCOUNTING_URI}/CurentTotals/{DateTime.Parse(dateFrom).AddDays(1).ToString("yyyy-MM-dd")}");
            return View(homeSummary);
        }

        public async Task<IActionResult> PreviousPeriod()
        {
            var date = DateTime.Parse(HttpContext.Session.GetString(Constants.DATE_FROM)).AddDays(-1).ToString("yyyy-MM-dd");
            var summaryModel = await GetHeaderSummary(date);

            var currentUrl = Request.Headers["Referer"].ToString();
            if (string.IsNullOrWhiteSpace(currentUrl))
                return RedirectToAction("Index");

            return Redirect(currentUrl);
        }

        public async Task<IActionResult> CurrentPeriod()
        {
            await GetHeaderSummary(DateTimeEast.Now.ToString("yyyy-MM-dd"));
            var currentUrl = Request.Headers["Referer"].ToString();
            if (string.IsNullOrWhiteSpace(currentUrl))
                return RedirectToAction("Index");

            return Redirect(currentUrl);
        }

        public async Task<IActionResult> NextPeriod()
        {
            var date = DateTime.Parse(HttpContext.Session.GetString(Constants.DATE_TO)).AddDays(1).ToString("yyyy-MM-dd");
            var summaryModel = await GetHeaderSummary(date);

            var currentUrl = Request.Headers["Referer"].ToString();
            if (string.IsNullOrWhiteSpace(currentUrl))
                return RedirectToAction("Index");

            return Redirect(currentUrl);
        }

        private async Task<MiniSummaryModel?> GetHeaderSummary(string date)
        {
            var summaryModel = await _httpHelper.Get<MiniSummaryModel?>($"{Constants.ACCOUNTING_URI}/GetHeaderSummary/{date}");
            if (summaryModel != null)
            {
                HttpContext.Session.SetString(Constants.DATE_FROM, summaryModel.PeriodFrom.ToString("MMM dd, yyyy"));
                HttpContext.Session.SetString(Constants.DATE_TO, summaryModel.PeriodTo.ToString("MMM dd, yyyy"));
                HttpContext.Session.SetString(Constants.CHEQUING, summaryModel.Chequing.ToString());
                HttpContext.Session.SetString(Constants.SAVINGS, summaryModel.Savings.ToString());
            }
            else
            {
                HttpContext.Session.Remove(Constants.DATE_FROM);
                HttpContext.Session.Remove(Constants.DATE_TO);
                HttpContext.Session.Remove(Constants.CHEQUING);
                HttpContext.Session.Remove(Constants.SAVINGS);
            }
            return summaryModel;
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
