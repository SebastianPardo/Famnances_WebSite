using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Famnances.Models.ViewModels.Introduction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Famnances.Models.ViewModels.Introduction.DiscountViewModel;
using static Famnances.Models.ViewModels.Introduction.IncomeViewModel;
using static Famnances.Models.ViewModels.Introduction.SavingsViewModel;

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
        #region Index PeriodSelector
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
        #endregion

        #region Income
        public async Task<ActionResult> Incomes(Guid periodId)
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            user.Period = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{periodId}");
            user.PeriodId = user.Period.Id;
            await _httpHelper.Put($"{Constants.USER_URI}/{accountId}", user);

            ViewBag.Periods = await _utilities.GetPeriodDropdown(user.Language);
            IncomeViewModel model = new IncomeViewModel
            {
                Incomes = new List<Income>(),
                Total = 0,
                PeriodId = periodId,
                Period = await _utilities.GetPeriodName(user.Language, user.Period)
            };

            return View(model);
        }

        public async Task<ActionResult> AddIncome(IncomeViewModel model)
        {
            var culture = Thread.CurrentThread.CurrentUICulture.ToString();
            var periodFrom = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{model.NewIncome.ValuePeriodId}");
            var periodTo = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{model.PeriodId}");

            model.NewIncome.Period = await _utilities.GetPeriodName(culture, periodFrom);
            model.Incomes = model.Incomes ?? new List<Income>();
            model.Incomes.Add(model.NewIncome);

            model.Total += _utilities.GetValueByPeriod(model.NewIncome.Value, periodFrom.Code, periodTo.Code);
            ModelState.Clear();

            ViewBag.Periods = await _utilities.GetPeriodDropdown(culture);
            return View(nameof(Incomes), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveIncomes(IncomeViewModel model)
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
                        PayablePeriodId = income.PayablePeriodId,
                        ValuePeriodId = income.ValuePeriodId,
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

            return RedirectToAction(nameof(Discounts),
                new DiscountViewModel
                {
                    Total = model.Total,
                    PeriodId = model.PeriodId,
                    Period = model.Period
                });
        }
        #endregion

        #region Discount
        public async Task<ActionResult> Discounts(DiscountViewModel model)
        {
            var incomes = await _httpHelper.Get<List<FixedIncome>>(Constants.FIXED_INCOMES_URI);
            ViewBag.Incomes = new SelectList(incomes, "Id", "Description");
            model.IncomeDiscounts = new List<Discount>();
            TempData["Total"] = model.Total;
            return View(model);
        }
        public async Task<ActionResult> AddDiscounts(DiscountViewModel model)
        {
            model.Total = Convert.ToDecimal(TempData["Total"]);
            TempData["Total"] = model.Total;

            var userPeriod = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{model.PeriodId}");

            Dictionary<Guid, decimal> incomesAfterPrevDicounts = new Dictionary<Guid, decimal>();

            model.IncomeDiscounts = model.IncomeDiscounts ?? new List<Discount>();
            if (model.IncomeDiscount.IsPrediscount)
                model.IncomeDiscounts.Insert(0, model.IncomeDiscount);
            else
                model.IncomeDiscounts.Add(model.IncomeDiscount);

            foreach (var discount in model.IncomeDiscounts)
            {
                foreach (var incomeId in discount.IncomeIds)
                {
                    var income = await _httpHelper.Get<Income>($"{Constants.FIXED_INCOMES_URI}/{incomeId}");
                    var payablePeriod = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{income.PayablePeriodId}");
                    var incomePeriod = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{income.ValuePeriodId}");
                    if (!incomesAfterPrevDicounts.ContainsKey(incomeId))
                        incomesAfterPrevDicounts.Add(incomeId, income.Value);

                    var discounPeriodValue = discount.ByPayablePeriod ? payablePeriod.Code : userPeriod.Code;
                    var incomeByPeriod = _utilities.GetValueByPeriod(income.Value, incomePeriod.Code, discounPeriodValue);

                    decimal discountValue = 0;
                    if (discount.IsPrediscount)
                    {
                        discountValue = discount.IsPercentage ? incomeByPeriod * discount.Value / 100 : discount.Value;

                        var incomeValue = _utilities.GetValueByPeriod(discountValue, discounPeriodValue, incomePeriod.Code);
                        incomesAfterPrevDicounts[incomeId] -= incomeValue;
                    }
                    else
                    {
                        incomeByPeriod = _utilities.GetValueByPeriod(incomesAfterPrevDicounts[incomeId], incomePeriod.Code, discounPeriodValue);
                        discountValue = discount.IsPercentage ? incomeByPeriod * discount.Value / 100 : discount.Value;
                    }

                    model.Total -= _utilities.GetValueByPeriod(discountValue, discounPeriodValue, userPeriod.Code);
                }
            }
            var incomes = await _httpHelper.Get<List<FixedIncome>>(Constants.FIXED_INCOMES_URI);
            model.IncomeDiscount = new Discount();
            ModelState.Clear();

            ViewBag.Incomes = new SelectList(incomes, "Id", "Description");
            return View(nameof(Discounts), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveDiscounts(DiscountViewModel model)
        {
            foreach (var discount in model.IncomeDiscounts)
            {
                IncomeDiscount incomeDiscount = new IncomeDiscount
                {
                    Active = true,
                    Description = discount.Description,
                    FixedIncomeByDiscount = discount.IncomeIds.Select(e =>
                        new FixedIncomeByDiscount
                        {
                            FixedIncomeId = e,
                            ByPayablePeriod = discount.ByPayablePeriod
                        }).ToList(),
                    IsPercentage = discount.IsPercentage,
                    IsTax = discount.IsTax,
                    IsPrediscount = discount.IsPrediscount,
                    Value = discount.Value,
                };
                incomeDiscount = await _httpHelper.Post<IncomeDiscount>(Constants.INCOME_DISCOUNTS_URI, incomeDiscount);
            }

            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            user.BudgetByPeriod = model.Total;
            await _httpHelper.Put<User>($"{Constants.USER_URI}/{user.Id}", user);

            return RedirectToAction(nameof(FixedExpenses),
                new FixedExpenseViewModel
                {
                    Total = model.Total,
                    PeriodId = model.PeriodId,
                    Period = model.Period
                });
        }
        #endregion

        #region FixedExpenses
        public async Task<ActionResult> FixedExpenses(FixedExpenseViewModel model)
        {
            var culture = Thread.CurrentThread.CurrentUICulture.ToString();
            ViewBag.Periods = await _utilities.GetPeriodDropdown(culture);
            model.Expenses = new List<FixedExpense>();
            model.Expense = new FixedExpense();
            return View(model);
        }

        public async Task<ActionResult> AddFixedExpense(FixedExpenseViewModel model)
        {
            var culture = Thread.CurrentThread.CurrentUICulture.ToString();
            var periodFrom = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{model.Expense.PeriodId}");
            var periodTo = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{model.PeriodId}");

            model.Period = await _utilities.GetPeriodName(culture, periodFrom);
            model.Expenses = model.Expenses ?? new List<FixedExpense>();
            model.Expenses.Add(model.Expense);

            model.Total -= _utilities.GetValueByPeriod(model.Expense.Value, periodFrom.Code, periodTo.Code);

            ViewBag.Periods = await _utilities.GetPeriodDropdown(culture);
            return View(nameof(FixedExpenses), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveFixedExpenses(FixedExpenseViewModel model)
        {
            foreach (var expense in model.Expenses)
            {
                expense.Active = true;
                expense.ShareOnHousehold = false;
                await _httpHelper.Post<FixedExpense>(Constants.FIXED_EXPENSES_URI, expense);
            }
            return RedirectToAction(nameof(Budgets),
                new BudgetViewModel
                {
                    Total = model.Total,
                    PeriodId = model.PeriodId,
                    Period = model.Period
                });
        }
        #endregion

        #region Budgets
        public async Task<ActionResult> Budgets(BudgetViewModel model)
        {
            model.Budgets = new List<ExpensesBudget>();
            return View(model);
        }

        public async Task<ActionResult> AddBudget(BudgetViewModel model)
        {
            if (model.Budgets == null)
                model.Budgets = new List<ExpensesBudget>();

            model.Budgets.Add(model.Budget);
            model.Total -= model.Budget.Value;
            ModelState.Clear();
            return View(nameof(Budgets), model);
        }

        public async Task<ActionResult> SaveBudgets(BudgetViewModel model)
        {
            foreach (var budget in model.Budgets)
            {
                var budgetType = await _httpHelper.Get<ExpensesBudgetType>($"{Constants.BUDGET_TYPES_URI}/GetByCode/PER");
                budget.BudgetTypeId = budgetType.Id;
                await _httpHelper.Post(Constants.BUDGETS_URI, budget);
            }
            return RedirectToAction(nameof(Savings), new SavingsViewModel
            {
                PeriodId = model.PeriodId,
                Period = model.Period,
                Total = model.Total
            });
        }
        #endregion

        #region Savings
        public async Task<ActionResult> Savings(SavingsViewModel model)
        {
            model.Pockets = new List<SavingPocket>();
            return View(model);
        }

        public async Task<ActionResult> AddPocket(SavingsViewModel model)
        {
            if (model.Pockets == null)
                model.Pockets = new List<SavingPocket> { model.Pocket };
            else
                model.Pockets.Add(model.Pocket);

            if (model.Pocket.FrecuentDeposits)
                model.Total -= model.Pocket.FrecuentValue.Value;

            ModelState.Clear();
            return View(nameof(Savings), model);
        }

        public async Task<ActionResult> SavePocket(SavingsViewModel model)
        {
            foreach (var pocket in model.Pockets)
            {
                var savingPocket = new SavingsPocket
                {
                    IsActive = true,
                    ChallengeValue = pocket.ChallengeValue,
                    Name = pocket.Name,
                    ShareOnHousehold = false,
                    Total = pocket.Total,
                };
                savingPocket = await _httpHelper.Post<SavingsPocket>(Constants.SAVINGS_POCKETS_URI, savingPocket);

                if (model.Pocket.FrecuentDeposits)
                {
                    FixedSaving fixedSaving = new FixedSaving
                    {
                        IsActive = true,
                        PeriodicityId = model.PeriodId,
                        SavingSourceId = (await _httpHelper.Get<SavingSource>($"{Constants.SAVING_SOURCES_URI}/OTHER")).Id,
                        SavingsPocketId = savingPocket.Id,
                        Value = model.Pocket.FrecuentValue.Value
                    };
                    await _httpHelper.Post<FixedSaving>(Constants.FIXED_SAVINGS_URI, fixedSaving);
                }
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        #endregion

        private decimal ValidateOverBudget(decimal total, decimal substractValue)
        {
            if (total - substractValue < 0)
            {
                TempData["Error"] = "That value es out of your general budget";
                return total;
            }
            else
            {
                return total - substractValue;
            }
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
