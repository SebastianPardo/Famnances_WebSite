using Famnances.Core.Security.Authorization;
using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Famnances.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Famnances.Controllers
{
    [ServiceFilter(typeof(AuthorizeAttribute))]
    public class SavingsController : Controller
    {
        IHttpHelper _httpHelper;
        ILanguageHelper _utilities;

        public SavingsController(IHttpHelper httpHelper, ILanguageHelper utilities)
        {
            _httpHelper = httpHelper;
            _utilities = utilities;
        }


        [ServiceFilter(typeof(HeaderSummaryFilter))]
        public async Task<IActionResult> Index()
        {
            var from = HttpContext.Session.GetString(Constants.DATE_FROM);
            var to = HttpContext.Session.GetString(Constants.DATE_TO);
            var savings = await _httpHelper.Get<List<SavingRecord>>($"{Constants.SAVINGS_URI}/{from}/{to}");
            return View(savings);
        }

        // GET: SavingRecords/Create
        public async Task<IActionResult> Create()
        {
            SavingTransactionViewModel model = new SavingTransactionViewModel
            {
                FixedSavings = await _httpHelper.Get<List<FixedSaving>>($"{Constants.FIXED_SAVINGS_URI}")
            };
            var pockets = await _httpHelper.Get<List<SavingsPocket>>($"{Constants.SAVINGS_POCKETS_URI}");
            ViewData["SavingsPocketId"] = new SelectList(pockets, "Id", "Name");
            var savingSources = await _httpHelper.Get<List<SavingSource>>($"{Constants.SAVING_SOURCES_URI}");
            ViewData["SavingsSources"] = new SelectList(savingSources, "Code", "Name");
            return View(model);
        }

        // POST: SavingRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SavingTransactionViewModel model)
        {
            SavingRecord savingRecord = model.SavingTransaction;
            var pocket = await _httpHelper.Get<SavingsPocket>($"{Constants.SAVINGS_POCKETS_URI}/{savingRecord.SavingsPocketId}");

            if (ModelState.IsValid)
            {
                if (model.SavingSource != "OTHER" && !savingRecord.IsExpense)
                {
                    var budget = await _httpHelper.Get<List<ExpensesBudget>>($"{Constants.BUDGETS_URI}/GetByType/SAV");
                    Outflow outflow = new Outflow
                    {
                        Id = Guid.NewGuid(),
                        Description = $"Transfer to savings {savingRecord.Description} {pocket.Name}",
                        ExpenseBudgetId = budget.First().Id,
                        TransactionDate = savingRecord.TransactionDate,
                        Value = savingRecord.Value
                    };
                    outflow = await _httpHelper.Post<Outflow>($"{Constants.OUTFLOWS_URI}", outflow);
                }
                if(savingRecord.IsExpense && model.TranferToChequing)
                {
                    Inflow inflow = new Inflow
                    {
                        Id = Guid.NewGuid(),
                        Description = $"Transfer from Savings {savingRecord.Description} {pocket.Name}",
                        TransactionDate = savingRecord.TransactionDate,
                        Value = savingRecord.Value
                    };
                    inflow = await _httpHelper.Post<Inflow>($"{Constants.INFLOWS_URI}", new IncomeTransactionModel { Income = inflow });
                }

                savingRecord.Id = Guid.NewGuid();
                savingRecord = await _httpHelper.Post<SavingRecord>($"{Constants.SAVINGS_URI}", savingRecord);
                return RedirectToAction(nameof(Index));
            }

            var pockets = await _httpHelper.Get<List<SavingsPocket>>($"{Constants.SAVINGS_POCKETS_URI}");
            ViewData["SavingsPocketId"] = new SelectList(pockets, "Id", "Name", savingRecord.SavingsPocketId);

            var savingSources = await _httpHelper.Get<List<SavingSource>>($"{Constants.SAVING_SOURCES_URI}");
            ViewData["SavingsSources"] = new SelectList(savingSources, "Code", "Name", model.SavingSource);

            return View(savingRecord);
        }

        // GET: SavingRecords/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savingRecord = await _httpHelper.Get<SavingRecord>($"{Constants.SAVINGS_URI}/{id}");
            if (savingRecord == null)
            {
                return NotFound();
            }
            var pockets = await _httpHelper.Get<List<SavingsPocket>>($"{Constants.SAVINGS_POCKETS_URI}");
            ViewData["SavingsPocketId"] = new SelectList(pockets, "Id", "Name", savingRecord.SavingsPocketId);
            return View(savingRecord);
        }

        // POST: SavingRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Description,IsExpense,IsEmergency,Value,TransactionDate,SavingsPocketId")] SavingRecord savingRecord)
        {
            if (id != savingRecord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _httpHelper.Put($"{Constants.SAVINGS_URI}/{id}", savingRecord);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _httpHelper.Get<SavingRecord>($"{Constants.SAVINGS_URI}/{id}") == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var pockets = await _httpHelper.Get<List<SavingsPocket>>($"{Constants.SAVINGS_POCKETS_URI}");
            ViewData["SavingsPocketId"] = new SelectList(pockets, "Id", "Name", savingRecord.SavingsPocketId);
            return View(savingRecord);
        }

        // POST: SavingRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var savingRecord = await _httpHelper.Get<SavingRecord>($"{Constants.SAVINGS_URI}/{id}");
            if (savingRecord != null)
            {
                await _httpHelper.Delete($"{Constants.SAVINGS_URI}/{id}");
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> IndexPockets()
        {
            var savingsPockets = await _httpHelper.Get<List<SavingsPocket>>($"{Constants.SAVINGS_POCKETS_URI}");
            return View(savingsPockets);
        }

        // GET: SavingsPockets/Create
        public IActionResult CreatePockets()
        {
            return View();
        }

        // POST: SavingsPockets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePockets([Bind("Id,Name,IsActive,ChallengeValue,Total,ShareOnHousehold")] SavingsPocket savingsPocket)
        {
            if (ModelState.IsValid)
            {
                savingsPocket.Id = Guid.NewGuid();
                savingsPocket = await _httpHelper.Post<SavingsPocket>($"{Constants.SAVINGS_POCKETS_URI}", savingsPocket);
                return RedirectToAction(nameof(IndexPockets));
            }
            return View(savingsPocket);
        }

        public async Task<IActionResult> DetailsPocket(Guid id)
        {
            var from = HttpContext.Session.GetString(Constants.DATE_FROM);
            var to = HttpContext.Session.GetString(Constants.DATE_TO);
            var savingsPocket = await _httpHelper.Get<SavingsPocket>($"{Constants.SAVINGS_POCKETS_URI}/{id}/{from}/{to}");
            return View(savingsPocket);
        }

        // GET: SavingsPockets/Edit/5
        public async Task<IActionResult> EditPockets(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var savingsPocket = await _httpHelper.Get<SavingsPocket>($"{Constants.SAVINGS_POCKETS_URI}/{id}");
            if (savingsPocket == null)
            {
                return NotFound();
            }
            return View(savingsPocket);
        }

        // POST: SavingsPockets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPockets(Guid id, [Bind("Id,Name,IsActive,ChallengeValue,Total,ShareOnHousehold")] SavingsPocket savingsPocket)
        {
            if (id != savingsPocket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _httpHelper.Put($"{Constants.SAVINGS_POCKETS_URI}/{id}", savingsPocket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _httpHelper.Get<SavingsPocket>($"{Constants.SAVINGS_POCKETS_URI}/{id}") == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(IndexPockets));
            }
            return View(savingsPocket);
        }


        // POST: SavingsPockets/Delete/5
        [HttpPost, ActionName("DeletePockets")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePockets(Guid id)
        {
            var savingsPocket = await _httpHelper.Get<SavingsPocket>($"{Constants.SAVINGS_POCKETS_URI}/{id}");
            if (savingsPocket != null)
            {
                await _httpHelper.Delete<SavingsPocket>($"{Constants.SAVINGS_POCKETS_URI}/{id}");
            }
            return RedirectToAction(nameof(IndexPockets));
        }

        [HttpGet]
        public async Task<IActionResult> IndexFixed()
        {
            var fixedSaving = await _httpHelper.Get<List<FixedSaving>>($"{Constants.FIXED_SAVINGS_URI}");
            return View(fixedSaving);
        }

        [HttpGet]
        // GET: SavingsPockets/Create
        public async Task<IActionResult> CreateFixed()
        {
            var savingSources = await _httpHelper.Get<List<SavingSource>>($"{Constants.SAVING_SOURCES_URI}");
            ViewBag.SavingSourceId = new SelectList(savingSources, "Id", "Name");
            ViewBag.Periods = await _utilities.GetPeriodDropdown(Thread.CurrentThread.CurrentUICulture.ToString());
            var savingsPockets = await _httpHelper.Get<List<SavingsPocket>>($"{Constants.SAVINGS_POCKETS_URI}");
            ViewBag.SavingsPocketId = new SelectList(savingsPockets, "Id", "Name");
            return View();
        }

        // POST: SavingsPockets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFixed([Bind("Value,IsActive,EndDate,PeriodicityId,SavingsPocketId,SavingSourceId")] FixedSaving fixedSaving)
        {
            if (ModelState.IsValid)
            {
                fixedSaving.Id = Guid.NewGuid();
                fixedSaving = await _httpHelper.Post<FixedSaving>($"{Constants.FIXED_SAVINGS_URI}", fixedSaving);
                return RedirectToAction(nameof(IndexFixed));
            }
            var savingSources = await _httpHelper.Get<List<SavingSource>>($"{Constants.SAVING_SOURCES_URI}");
            ViewBag.SavingSourceId = new SelectList(savingSources, "Id", "Name");

            ViewBag.Periods = await _utilities.GetPeriodDropdown(Thread.CurrentThread.CurrentUICulture.ToString());
            var savingsPockets = await _httpHelper.Get<List<SavingsPocket>>($"{Constants.SAVINGS_POCKETS_URI}");
            ViewBag.SavingsPocketId = new SelectList(savingsPockets, "Id", "Name");
            return View(fixedSaving);
        }

        // GET: SavingsPockets/Edit/5
        public async Task<IActionResult> EditFixed(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fixedSaving = await _httpHelper.Get<FixedSaving>($"{Constants.FIXED_SAVINGS_URI}/{id}");
            if (fixedSaving == null)
            {
                return NotFound();
            }
            var savingSources = await _httpHelper.Get<List<SavingSource>>($"{Constants.SAVING_SOURCES_URI}");
            ViewBag.SavingSourceId = new SelectList(savingSources, "Id", "Name", fixedSaving.SavingSourceId); 
            
            ViewBag.Periods = await _utilities.GetPeriodDropdown(Thread.CurrentThread.CurrentUICulture.ToString());
            var savingsPockets = await _httpHelper.Get<List<SavingsPocket>>($"{Constants.SAVINGS_POCKETS_URI}");
            ViewBag.SavingsPocketId = new SelectList(savingsPockets, "Id", "Name", fixedSaving.SavingsPocketId);
            return View(fixedSaving);
        }

        // POST: SavingsPockets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFixed(Guid id, [Bind("Id,Value,IsActive,EndDate,PeriodicityId,SavingsPocketId,SavingSourceId")] FixedSaving fixedSaving)
        {
            if (id != fixedSaving.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _httpHelper.Put($"{Constants.FIXED_SAVINGS_URI}/{id}", fixedSaving);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _httpHelper.Get<FixedSaving>($"{Constants.FIXED_SAVINGS_URI}/{id}") == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(IndexFixed));
            }
            var savingSources = await _httpHelper.Get<List<SavingSource>>($"{Constants.SAVING_SOURCES_URI}");
            ViewBag.SavingSourceId = new SelectList(savingSources, "Id", "Name", fixedSaving.SavingSourceId);
            
            ViewBag.Periods = await _utilities.GetPeriodDropdown(Thread.CurrentThread.CurrentUICulture.ToString());
            var savingsPockets = await _httpHelper.Get<List<SavingsPocket>>($"{Constants.SAVINGS_POCKETS_URI}");
            ViewBag.SavingsPocketId = new SelectList(savingsPockets, "Id", "Name", fixedSaving.SavingsPocketId);
            return View(fixedSaving);
        }


        // POST: SavingsPockets/Delete/5
        [HttpPost, ActionName("DeleteFixed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFixed(Guid id)
        {
            var fixedSaving = await _httpHelper.Get<FixedSaving>($"{Constants.FIXED_SAVINGS_URI}/{id}");
            if (fixedSaving != null)
            {
                await _httpHelper.Delete<FixedSaving>($"{Constants.FIXED_SAVINGS_URI}/{id}");
            }
            return RedirectToAction(nameof(IndexFixed));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransferFixed(Guid id)
        {
            var fixedSaving = await _httpHelper.Get<FixedSaving>($"{Constants.FIXED_SAVINGS_URI}/{id}");

            if (fixedSaving.SavingSource.Code != "OTHER")
            {
                var budget = await _httpHelper.Get<List<ExpensesBudget>>($"{Constants.BUDGETS_URI}/GetByType/SAV");
                Outflow outflow = new Outflow
                {
                    Id = Guid.NewGuid(),
                    Description = $"Scheduled transfered - to {fixedSaving.SavingsPocket.Name} saving pocket",
                    ExpenseBudgetId = budget.First().Id,
                    TransactionDate = DateTimeEast.Now,
                    Value = fixedSaving.Value
                };
                outflow = await _httpHelper.Post<Outflow>($"{Constants.OUTFLOWS_URI}", outflow);
            }

            SavingRecord savingRecord = new SavingRecord
            {
                Id = Guid.NewGuid(),
                Description = $"Scheduled transfered - From {fixedSaving.SavingSource.Name} to {fixedSaving.SavingsPocket.Name}",
                IsExpense = false ,
                TransactionDate = DateTimeEast.Now,
                SavingsPocketId = fixedSaving.SavingsPocketId,
                IsEmergency = false ,
                Value = fixedSaving.Value                
            };
            await _httpHelper.Post<SavingRecord>($"{Constants.SAVINGS_URI}", savingRecord);

            return RedirectToAction(nameof(Index));

        }
    }
}
