using Famnances.Core.Security.Authorization;
using Famnances.Core.Utils.Helpers;
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

        [AllowAnonymous]
        public async Task<ActionResult> Language()
        {
            return View();
        }

        public async Task<ActionResult> Index()
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");

            ViewBag.UserName = user.LegalName;
            ViewBag.Photo = user.Photo;
            return View();
        }


        #region Index PeriodSelector
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

        [HttpPost]
        public async Task<ActionResult> ChangePeriod(Guid periodId)
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");

            user.PeriodId = periodId != Guid.Empty ? periodId : user.PeriodId;

            await _httpHelper.Put($"{Constants.USER_URI}/{accountId}", user);
            return RedirectToAction(nameof(Incomes));
        }
        #endregion

        #region Income
        public async Task<ActionResult> Incomes()
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");

            ViewBag.Periods = await _utilities.GetPeriodDropdown(user.Language);
            IncomeViewModel model = new IncomeViewModel
            {
                Total = 0,
                PeriodId = user.PeriodId,
                Period = await _utilities.GetPeriodName(user.Language, user.Period),
                Incomes = new List<Income>()
            };

            var fixedIncomes = await _httpHelper.Get<List<FixedIncome>>(Constants.FIXED_INCOMES_URI);
            if (fixedIncomes != null && fixedIncomes.Count > 0)
            {
                model.Incomes = fixedIncomes.Select(e => new Income(e)).ToList();
            }

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

        public async Task<ActionResult> RemoveIncome(int index, IncomeViewModel model)
        {
            var income = model.Incomes[index];

            var culture = Thread.CurrentThread.CurrentUICulture.ToString();
            var periodFrom = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{income.ValuePeriodId}");
            var periodTo = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{model.PeriodId}");

            model.Incomes.Remove(income);

            model.Total -= _utilities.GetValueByPeriod(income.Value, periodFrom.Code, periodTo.Code);
            ModelState.Clear();

            ViewBag.Periods = await _utilities.GetPeriodDropdown(culture);
            return View(nameof(Incomes), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveIncomes(IncomeViewModel model)
        {
            var fixedIncomes = await _httpHelper.Get<List<FixedIncome>>(Constants.FIXED_INCOMES_URI);
            if (fixedIncomes != null && fixedIncomes.Count > 0)
            {
                await _httpHelper.Put($"{Constants.FIXED_INCOMES_URI}/Inactive", null);
            }

            if (model.Incomes != null)
            {
                foreach (var income in model.Incomes)
                {
                    if (income.Type == IncomeType.Fixed)
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
            }

            return RedirectToAction(nameof(Discounts));
        }
        #endregion

        #region Discount
        public async Task<ActionResult> Discounts()
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            var culture = Thread.CurrentThread.CurrentUICulture.ToString();

            var incomes = await _httpHelper.Get<List<FixedIncome>>(Constants.FIXED_INCOMES_URI);
            ViewBag.Incomes = new SelectList(incomes, "Id", "Description");

            DiscountViewModel model = new DiscountViewModel
            {
                Total = user.BudgetByPeriod,
                PeriodId = user.PeriodId,
                Period = await _utilities.GetPeriodName(culture, user.Period),
                IncomeDiscounts = new List<Discount>()
            };

            var discounts = await _httpHelper.Get<List<IncomeDiscount>>(Constants.INCOME_DISCOUNTS_URI);
            if (discounts != null && discounts.Count > 0)
            {
                model.IncomeDiscounts = discounts.Select(e => new Discount(e)).ToList();
            }

            return View(model);
        }
        public async Task<ActionResult> AddDiscounts(DiscountViewModel model)
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");

            var userPeriod = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{model.PeriodId}");
            model.IncomeDiscounts = model.IncomeDiscounts ?? new List<Discount>();

            if (model.IncomeDiscount.IsPrediscount)
                model.IncomeDiscounts.Insert(0, model.IncomeDiscount);
            else
                model.IncomeDiscounts.Add(model.IncomeDiscount);



            model.Total = await CalculateDiscounts(user.BudgetByPeriod, userPeriod.Code, model.IncomeDiscounts);

            var incomes = await _httpHelper.Get<List<FixedIncome>>(Constants.FIXED_INCOMES_URI);
            model.IncomeDiscount = new Discount();
            ModelState.Clear();

            ViewBag.Incomes = new SelectList(incomes, "Id", "Description");
            return View(nameof(Discounts), model);
        }

        public async Task<ActionResult> RemoveDiscount(int index, DiscountViewModel model)
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            var userPeriod = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{model.PeriodId}");
            var discount = model.IncomeDiscounts[index];

            model.IncomeDiscounts.Remove(discount);
            model.Total = await CalculateDiscounts(user.BudgetByPeriod, userPeriod.Code, model.IncomeDiscounts);

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
            var discounts = await _httpHelper.Get<List<IncomeDiscount>>(Constants.INCOME_DISCOUNTS_URI);
            if (discounts != null && discounts.Count > 0)
            {
                await _httpHelper.Put($"{Constants.INCOME_DISCOUNTS_URI}/Inactive", null);
            }

            if (model.IncomeDiscounts != null)
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
            }
            return RedirectToAction(nameof(FixedExpenses));
        }
        #endregion

        #region FixedExpenses
        public async Task<ActionResult> FixedExpenses()
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            var culture = Thread.CurrentThread.CurrentUICulture.ToString();

            FixedExpenseViewModel model = new FixedExpenseViewModel
            {
                Total = user.BudgetByPeriod,
                PeriodId = user.PeriodId,
                Period = await _utilities.GetPeriodName(culture, user.Period),
                Expenses = new List<FixedExpense>(),
                Expense = new FixedExpense()
            };

            var fixedExpenses = await _httpHelper.Get<List<FixedExpense>>(Constants.FIXED_EXPENSES_URI);
            if (fixedExpenses != null && fixedExpenses.Count > 0)
            {
                model.Expenses = fixedExpenses;
            }

            ViewBag.Periods = await _utilities.GetPeriodDropdown(culture);
            return View(model);
        }

        public async Task<ActionResult> AddFixedExpense(FixedExpenseViewModel model)
        {
            var culture = Thread.CurrentThread.CurrentUICulture.ToString();
            var periodFrom = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{model.Expense.PeriodId}");
            var periodTo = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{model.PeriodId}");

            decimal expenseValueByPeriod = _utilities.GetValueByPeriod(model.Expense.Value, periodFrom.Code, periodTo.Code);

            if (expenseValueByPeriod <= model.Total)
            {
                model.Total -= expenseValueByPeriod;
                model.Period = await _utilities.GetPeriodName(culture, periodFrom);
                model.Expense.Period = periodFrom;
                model.Expenses = model.Expenses ?? new List<FixedExpense>();
                model.Expenses.Add(model.Expense);
            }
            else
            {
                TempData[Constants.ERROR] = "Exceeded";
            }

            ViewBag.Periods = await _utilities.GetPeriodDropdown(culture);
            return View(nameof(FixedExpenses), model);
        }

        public async Task<ActionResult> RemoveExpense(int index, FixedExpenseViewModel model)
        {
            var expense = model.Expenses[index];

            var culture = Thread.CurrentThread.CurrentUICulture.ToString();
            var periodFrom = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{expense.PeriodId}");
            var periodTo = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{model.PeriodId}");

            model.Expenses.Remove(expense);

            model.Total += _utilities.GetValueByPeriod(expense.Value, periodFrom.Code, periodTo.Code);
            ModelState.Clear();

            ViewBag.Periods = await _utilities.GetPeriodDropdown(culture);
            return View(nameof(FixedExpenses), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveFixedExpenses(FixedExpenseViewModel model)
        {
            var fixedExpenses = await _httpHelper.Get<List<FixedExpense>>(Constants.FIXED_EXPENSES_URI);
            if (fixedExpenses != null && fixedExpenses.Count > 0)
            {
                await _httpHelper.Put($"{Constants.FIXED_EXPENSES_URI}/Inactive", null);
            }

            if (model.Expenses != null)
            {
                foreach (var expense in model.Expenses)
                {
                    expense.Active = true;
                    expense.ShareOnHousehold = false;
                    await _httpHelper.Post<FixedExpense>(Constants.FIXED_EXPENSES_URI, expense);
                }
            }

            TempData[Constants.TOTAL] = model.Total;
            return RedirectToAction(nameof(Budgets));
        }
        #endregion

        #region Budgets
        public async Task<ActionResult> Budgets()
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            var culture = Thread.CurrentThread.CurrentUICulture.ToString();

            BudgetViewModel model = new BudgetViewModel
            {
                Total = Convert.ToDecimal(TempData[Constants.TOTAL]),
                PeriodId = user.PeriodId,
                Period = await _utilities.GetPeriodName(culture, user.Period),
                Budgets = new List<ExpensesBudget>()
            };

            var budgets = await _httpHelper.Get<List<ExpensesBudget>>(Constants.BUDGETS_URI);
            if (budgets != null && budgets.Count > 0)
            {
                model.Budgets = budgets;
            }

            return View(model);
        }

        public async Task<ActionResult> AddBudget(BudgetViewModel model)
        {
            if (model.Budgets == null)
                model.Budgets = new List<ExpensesBudget>();

            if (model.Budget.Value <= model.Total)
            {
                model.Budgets.Add(model.Budget);
                model.Total -= model.Budget.Value;
            }
            else
            {
                TempData[Constants.ERROR] = "Exceeded";
            }

            ModelState.Clear();
            return View(nameof(Budgets), model);
        }

        public async Task<ActionResult> RemoveBudget(int index, BudgetViewModel model)
        {
            var budget = model.Budgets[index];
            model.Budgets.Remove(budget);
            model.Total += budget.Value;
            ModelState.Clear();
            return View(nameof(Budgets), model);
        }

        public async Task<ActionResult> SaveBudgets(BudgetViewModel model)
        {
            var budgets = await _httpHelper.Get<List<ExpensesBudget>>(Constants.BUDGETS_URI);
            if (budgets != null && budgets.Count > 0)
            {
                await _httpHelper.Put($"{Constants.BUDGETS_URI}/Inactive", null);
            }

            if (model.Budgets != null)
            {
                foreach (var budget in model.Budgets)
                {
                    var budgetType = await _httpHelper.Get<ExpensesBudgetType>($"{Constants.BUDGET_TYPES_URI}/GetByCode/PER");
                    budget.BudgetTypeId = budgetType.Id;
                    await _httpHelper.Post(Constants.BUDGETS_URI, budget);
                }
            }

            TempData[Constants.TOTAL] = model.Total;
            return RedirectToAction(nameof(Savings));
        }
        #endregion

        #region Savings
        public async Task<ActionResult> Savings()
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            var culture = Thread.CurrentThread.CurrentUICulture.ToString();

            SavingsViewModel model = new SavingsViewModel
            {
                Total = Convert.ToDecimal(TempData[Constants.TOTAL]),
                PeriodId = user.PeriodId,
                Period = await _utilities.GetPeriodName(culture, user.Period),
                Pockets = new List<SavingPocket>()
            };

            var pockets = await _httpHelper.Get<List<SavingPocket>>(Constants.SAVINGS_POCKETS_URI);
            if (pockets != null && pockets.Count > 0)
            {
                model.Pockets = pockets;
            }

            return View(model);
        }

        public async Task<ActionResult> AddPocket(SavingsViewModel model)
        {
            if (model.Pockets == null)
                model.Pockets = new List<SavingPocket>();

            if (model.Pocket.FrecuentDeposits && model.Pocket.FrecuentValue <= model.Total)
            {
                model.Total -= model.Pocket.FrecuentValue.Value;
                model.Pockets.Add(model.Pocket);
            }
            else if (!model.Pocket.FrecuentDeposits)
            {
                model.Pockets.Add(model.Pocket);
            }
            else
            {
                TempData[Constants.ERROR] = "Exceeded";
            }

            ModelState.Clear();
            return View(nameof(Savings), model);
        }

        public async Task<ActionResult> RemovePocket(int index, SavingsViewModel model)
        {
            var pocket = model.Pockets[index];
            model.Pockets.Remove(pocket);

            if (pocket.FrecuentDeposits)
                model.Total += pocket.FrecuentValue.Value;
            ModelState.Clear();

            return View(nameof(Savings), model);
        }

        public async Task<ActionResult> SavePocket(SavingsViewModel model)
        {
            var budgets = await _httpHelper.Get<List<ExpensesBudget>>(Constants.BUDGETS_URI);
            if (budgets != null && budgets.Count > 0)
            {
                await _httpHelper.Put($"{Constants.BUDGETS_URI}/Inactive", null);
            }

            if (model.Pockets != null)
            {
                decimal currentSavings = 0;
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
                            Value = model.Pocket.FrecuentValue.Value,
                            EndDate = DateTimeEast.Now.AddYears(1)
                        };
                        await _httpHelper.Post<FixedSaving>(Constants.FIXED_SAVINGS_URI, fixedSaving);
                    }
                    currentSavings += pocket.Total;
                }
                var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
                var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
                user.TotalSavings = currentSavings;
                await _httpHelper.Put<User>($"{Constants.USER_URI}/{user.Id}", user);
            }
            TempData[Constants.TOTAL] = model.Total;
            return RedirectToAction(nameof(Summary));
        }
        #endregion

        #region Summary
        public async Task<ActionResult> Summary()
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            var culture = Thread.CurrentThread.CurrentUICulture.ToString();
            var fixedSavings = await _httpHelper.Get<List<FixedSaving>>($"{Constants.FIXED_SAVINGS_URI}");

            SummaryViewModel summary = new SummaryViewModel
            {
                Period = await _utilities.GetPeriodName(culture, user.Period),
                Total = Convert.ToDecimal(TempData[Constants.TOTAL]),
                BudgetByPeriod = user.BudgetByPeriod,
                CurrentMoney = 0,
                Savings = user.TotalSavings,
                SavingsByPeriod = fixedSavings.Select(e => e.Value).Sum()
            };
            return View(summary);
        }

        [HttpPost]
        public async Task<ActionResult> EndIntroduction(decimal currentMoney)
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            user.TotalBudget = currentMoney;
            await _httpHelper.Put<User>($"{Constants.USER_URI}/{user.Id}", user);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        #endregion

        #region Helper Methods

        private async Task<decimal> CalculateDiscounts(decimal total, string userCode, List<Discount> discounts)
        {
            Dictionary<Guid, decimal> incomesAfterPrevDicounts = new Dictionary<Guid, decimal>();

            foreach (var discount in discounts)
            {
                foreach (var incomeId in discount.IncomeIds)
                {
                    var income = await _httpHelper.Get<Income>($"{Constants.FIXED_INCOMES_URI}/{incomeId}");
                    var payablePeriod = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{income.PayablePeriodId}");
                    var incomePeriod = await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/{income.ValuePeriodId}");
                    if (!incomesAfterPrevDicounts.ContainsKey(incomeId))
                        incomesAfterPrevDicounts.Add(incomeId, income.Value);

                    var discounPeriodValue = discount.ByPayablePeriod ? payablePeriod.Code : userCode;
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

                    total -= _utilities.GetValueByPeriod(discountValue, discounPeriodValue, userCode);
                }
            }

            return total;
        }
        #endregion
    }
}
