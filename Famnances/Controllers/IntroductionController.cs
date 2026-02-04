using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Famnances.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using static Famnances.Models.ViewModels.IntroductionIncomeViewModel;

namespace Famnances.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
    public class IntroductionController : Controller
    {
        IHttpHelper _httpHelper;
        ILanguageHelper _utilities;
        public IntroductionController(IHttpHelper httpHelper, ILanguageHelper utilities)
        {
            _httpHelper = httpHelper;
            _utilities = utilities;
        }
        public async Task<ActionResult> Language()
        {
            return View();
        }
        public async Task<ActionResult> Index()
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");

            ViewBag.UserName = user.LegalName;
            ViewBag.Photo = "https://images.rawpixel.com/image_png_800/cHJpdmF0ZS9sci9pbWFnZXMvd2Vic2l0ZS8yMDIzLTAyL3BmLWljb240LWppcjIwNjQtcG9yLTAzLWxjb3B5LnBuZw.png";
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

        public async Task<ActionResult> Incomes(Guid periodId)
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            user.Period = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{periodId}");
            user.PeriodId = user.Period.Id;
            await _httpHelper.Put($"{Constants.USER_URI}/{accountId}", user);

            ViewBag.Periods = await _utilities.GetPeriodDropdown(user.Language);
            IntroductionIncomeViewModel model = new IntroductionIncomeViewModel
            {
                Incomes = new List<Income>(),
                Total = 0,
                PeriodId = periodId,
                Period = await _utilities.GetPeriodName(user.Language, user.Period)
            };

            return View(model);
        }

        public async Task<ActionResult> AddIncome(IntroductionIncomeViewModel model)
        {
            var culture = Thread.CurrentThread.CurrentUICulture.ToString();
            var periodFrom = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{model.NewIncome.PeriodId}");
            var periodTo = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{model.PeriodId}");

            model.NewIncome.Period = await _utilities.GetPeriodName(culture, periodFrom);
            model.Incomes = model.Incomes ?? new List<Income>();
            model.Incomes.Add(model.NewIncome);

            model.Total += _utilities.GetValueByPeriod(model.NewIncome.Value, periodFrom.Code, periodTo.Code);

            ViewBag.Periods = await _utilities.GetPeriodDropdown(culture);
            return View("Incomes", model);
        }

        public async Task<ActionResult> SaveIncomes(IntroductionIncomeViewModel model)
        {
            foreach (var income in model.Incomes)
            {
                if (income.Type == "FIXED")
                {
                    FixedIncome fixedIncome = new FixedIncome
                    {
                        Active = true,
                        Description = income.Description,
                        FirstPayDate = income.FirstPayDate,
                        PayablePeriodId = income.PeriodId,
                        ShareOnHousehold = false,
                        Value = income.Value
                    };
                    await _httpHelper.Post<FixedIncome>(Constants.FIXED_INCOMES_URI, fixedIncome);
                }
            }

            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            user.BudgetByPeriod = model.Total;
            await _httpHelper.Put<User>($"{Constants.USER_URI}/{user.Id}", user);

            return RedirectToAction("FixedExpenses", 
                new IntroductionFixedExpenseViewModel { 
                    Total = model.Total, 
                    PeriodId = model.PeriodId, 
                    Period = model.Period
                });
        }

        public async Task<ActionResult> FixedExpenses(IntroductionFixedExpenseViewModel model)
        {
            var culture = Thread.CurrentThread.CurrentUICulture.ToString();
            ViewBag.Periods = await _utilities.GetPeriodDropdown(culture);
            model.Expenses = new List<FixedExpense>();
            model.Expense = new FixedExpense();
            return View(model);
        }

        public async Task<ActionResult> AddFixedExpense(IntroductionFixedExpenseViewModel model)
        {
            var culture = Thread.CurrentThread.CurrentUICulture.ToString();
            var periodFrom = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{model.Expense.PeriodId}");
            var periodTo = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{model.PeriodId}");

            model.Period = await _utilities.GetPeriodName(culture, periodFrom);
            model.Expenses = model.Expenses ?? new List<FixedExpense>();
            model.Expenses.Add(model.Expense);

            model.Total -= _utilities.GetValueByPeriod(model.Expense.Value, periodFrom.Code, periodTo.Code);

            ViewBag.Periods = await _utilities.GetPeriodDropdown(culture);
            return View("FixedExpenses", model);
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
