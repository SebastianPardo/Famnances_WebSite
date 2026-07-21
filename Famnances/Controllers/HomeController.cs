using Famnances.Core.Errors;
using Famnances.Core.Security.Authorization;
using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
using Famnances.Helpers.Interfaces;
using Famnances.Models.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using Constants = Famnances.Helpers.Constants;

namespace Famnances.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IHttpHelper _httpHelper;
        ILanguageHelper _languageHelper;

        public HomeController(ILogger<HomeController> logger, IHttpHelper httpHelper, ILanguageHelper languageHelper)
        {
            _logger = logger;
            _httpHelper = httpHelper;
            _languageHelper = languageHelper;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Landing()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            var culture = Thread.CurrentThread.CurrentUICulture.ToString();

            var dateFrom = HttpContext.Session.GetString(Constants.DATE_FROM) ?? DateTimeEast.Now.ToString("yyyy-MM-dd");
            MiniSummaryModel? miniSummaryModel = await GetHeaderSummary(dateFrom);

            if (miniSummaryModel == null)
            {
                return RedirectToAction(nameof(ClosePeriod));
            }

            var homeSummary = await _httpHelper.Get<HomeViewModel>($"{Constants.ACCOUNTING_URI}/PeriodSummary/{DateTime.Parse(dateFrom).AddDays(1).ToString("yyyy-MM-dd")}");
            homeSummary.PeriodName = await _languageHelper.GetPeriodName(culture, user.Period);
            homeSummary.ToBeClosed = miniSummaryModel.ToBeClosed;
            return View(homeSummary);
        }

        public async Task<IActionResult> VeryFirstPeriod()
        {
            await _httpHelper.Post<Guid>($"{Constants.ACCOUNTING_URI}/ClosePeriod", new List<RemainderBalance>());
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ClosePeriod()
        {
            var period = await _httpHelper.Get<TotalsByPeriod>($"{Constants.TOTALSBYPERIOD_URI}/GetLastPeriod");
            GetHeaderSummary(period.PeriodDateStart.ToString("yyyy-MM-dd"));

            var summary = await _httpHelper.Get<List<SummaryBudgetModel>>($"{Constants.BUDGETS_URI}/GetSummary");
            List<RemainderBalance> remainderBalance = summary.Select(e =>
                new RemainderBalance
                {
                    BudgetId = e.Id,
                    BudgetName = e.Name,
                    Remainder = e.Budget - e.Spent,
                    BudgetBalanceId = e.BudgetPeriodBalanceId
                }).Where(e => e.Remainder > 0).ToList();

            var savingPockets = await _httpHelper.Get<List<SavingsPocket>>(Constants.SAVINGS_POCKETS_URI);
            ViewBag.MoveTo = new SelectList(savingPockets, "Id", "Name");

            if (remainderBalance == null || remainderBalance.Count == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(remainderBalance);
        }

        [HttpPost]
        public async Task<IActionResult> ClosePeriod(List<RemainderBalance> remainderBalance)
        {
            await _httpHelper.Post<Guid>($"{Constants.ACCOUNTING_URI}/ClosePeriod", remainderBalance);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PreviousPeriod()
        {
            var date = DateTime.Parse(HttpContext.Session.GetString(Constants.DATE_FROM)).AddDays(-1).ToString("yyyy-MM-dd");
            var summaryModel = await GetHeaderSummary(date);

            var currentUrl = Request.Headers["Referer"].ToString();
            if (string.IsNullOrWhiteSpace(currentUrl))
                return RedirectToAction(nameof(Index));

            return Redirect(currentUrl);
        }

        public async Task<IActionResult> CurrentPeriod()
        {
            await GetHeaderSummary(DateTimeEast.Now.ToString("yyyy-MM-dd"));
            var currentUrl = Request.Headers["Referer"].ToString();
            if (string.IsNullOrWhiteSpace(currentUrl))
                return RedirectToAction(nameof(Index));

            return Redirect(currentUrl);
        }

        public async Task<IActionResult> NextPeriod()
        {
            var date = DateTime.Parse(HttpContext.Session.GetString(Constants.DATE_TO)).AddDays(1).ToString("yyyy-MM-dd");
            var summaryModel = await GetHeaderSummary(date);

            var currentUrl = Request.Headers["Referer"].ToString();
            if (string.IsNullOrWhiteSpace(currentUrl))
                return RedirectToAction(nameof(Index));

            return Redirect(currentUrl);
        }

        private async Task<MiniSummaryModel?> GetHeaderSummary(string date)
        {
            var summaryModel = await _httpHelper.Get<MiniSummaryModel?>($"{Constants.ACCOUNTING_URI}/PeriodMiniSummary/{date}");
            if (summaryModel != null)
            {
                HttpContext.Session.SetString(Constants.DATE_FROM, summaryModel.PeriodFrom.ToString("yyyy-MM-dd"));
                HttpContext.Session.SetString(Constants.DATE_TO, summaryModel.PeriodTo.ToString("yyyy-MM-dd"));
                HttpContext.Session.SetString(Constants.CHEQUING, summaryModel.Chequing.ToString());
                HttpContext.Session.SetString(Constants.SAVINGS, summaryModel.Savings.ToString());
            }
            return summaryModel;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Offline()
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
