using Famnances.DataCore.Entities;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Famnances.Controllers
{
    public class IntroductionController : Controller
    {
        IHttpHelper _httpHelper;
        IUtilities _utilities;
        public IntroductionController(IHttpHelper httpHelper, IUtilities utilities)
        {
            _httpHelper = httpHelper;
            _utilities = utilities;
        }
        public async Task<ActionResult> Language()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> PeriodSelector()
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");

            switch (Thread.CurrentThread.CurrentUICulture.ToString())
            {
                case "es-CO":
                    user.Language = "ES";
                    break;
                case "en-CA":
                    user.Language = "EN";
                    break;
                case "fr-CA":
                    user.Language = "FR";
                    break;
                default:
                    user.Language = "EN";
                    break;
            }

            await _httpHelper.Put($"{Constants.USER_URI}/{accountId}", user);
            List<Period> periods = await _httpHelper.Get<List<Period>>(Constants.PERIODS_URI);
            return View(periods);
        }

        public async Task<ActionResult> Incomes(string periodId)
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            user.PeriodId = (await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{periodId}")).Id;
            await _httpHelper.Put($"{Constants.USER_URI}/{accountId}", user);

            ViewBag.Periods = await _utilities.GetPeriodDropdown(user.Language);

            return View();
        }

        // GET: IntroductionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IntroductionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
