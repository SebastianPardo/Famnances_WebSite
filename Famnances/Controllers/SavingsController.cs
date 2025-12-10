using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using Famnances.DataCore.ServicesModels;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Famnances.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Famnances.Controllers
{
    public class SavingsController : Controller
    {
        IHttpHelper _httpHelper;

        public SavingsController(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
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
            var pockets = await _httpHelper.Get<List<SavingsPocket>>($"{Constants.SAVINGS_POCKETS_URI}");
            ViewData["SavingsPocketId"] = new SelectList(pockets, "Id", "Name");
            ViewData["SavingsSources"] = new SelectList(
                new[] {
                    new { Id = "CASH", Name = "Other" },
                    new { Id = "CHE", Name = "Chequing" }
                }, "Id", "Name");
            return View();
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
                if (model.SavingSource != "CASH" && !savingRecord.IsExpense)
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
                await _httpHelper.Delete<SavingRecord>($"{Constants.SAVINGS_URI}/{id}");
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
            var savingsPockets = await _httpHelper.Get<List<FixedSaving>>($"{Constants.FIXED_SAVINGS_URI}");
            return View(savingsPockets);
        }

        [HttpGet]
        // GET: SavingsPockets/Create
        public async Task<IActionResult> CreateFixed()
        {
            var savingSources = await _httpHelper.Get<List<SavingSource>>($"{Constants.SAVING_SOURCES_URI}");
            ViewBag.SavingSourceId = new SelectList(savingSources, "Id", "Name");
            var periods = await _httpHelper.Get<List<Period>>($"{Constants.PERIODS_URI}");
            ViewBag.PeriodicityId = new SelectList(periods, "Id", "Name");
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
            var periods = await _httpHelper.Get<List<Period>>($"{Constants.PERIODS_URI}");
            ViewBag.PeriodicityId = new SelectList(periods, "Id", "Name");
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
            var periods = await _httpHelper.Get<List<Period>>($"{Constants.PERIODS_URI}");
            ViewBag.PeriodicityId = new SelectList(periods, "Id", "Name", fixedSaving.PeriodicityId);
            var savingsPockets = await _httpHelper.Get<List<SavingsPocket>>($"{Constants.SAVINGS_POCKETS_URI}");
            ViewBag.SavingsPocketId = new SelectList(savingsPockets, "Id", "Name", fixedSaving.SavingsPocketId);
            return View(fixedSaving);
        }

        // POST: SavingsPockets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPockets(Guid id, [Bind("Id,Value,IsActive,EndDate,PeriodicityId,SavingsPocketId,SavingSourceId")] FixedSaving fixedSaving)
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
            var periods = await _httpHelper.Get<List<Period>>($"{Constants.PERIODS_URI}");
            ViewBag.PeriodicityId = new SelectList(periods, "Id", "Name", fixedSaving.PeriodicityId);
            var savingsPockets = await _httpHelper.Get<List<SavingsPocket>>($"{Constants.SAVINGS_POCKETS_URI}");
            ViewBag.SavingsPocketId = new SelectList(savingsPockets, "Id", "Name", fixedSaving.SavingsPocketId);
            return View(fixedSaving);
        }


        // POST: SavingsPockets/Delete/5
        [HttpPost, ActionName("DeletePockets")]
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
    }
}
