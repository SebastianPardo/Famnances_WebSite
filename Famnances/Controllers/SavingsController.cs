using Famnances.DataCore.Data;
using Famnances.DataCore.Entities;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Famnances.WebSite.Controllers
{
    public class SavingsController : Controller
    {
        IHttpHelper _httpHelper;

        public SavingsController(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        public async Task<IActionResult> Index()
        {

            var savings = await _httpHelper.Get<List<SavingRecord>>($"{Constants.SAVINGS_URI}");
            return View(savings);
        }

        // GET: SavingRecords/Create
        public async Task<IActionResult> Create()
        {
            var pockets = await _httpHelper.Get<List<SavingsPocket>>($"{Constants.SAVINGS_POCKETS_URI}");
            ViewData["SavingsPocketId"] = new SelectList(pockets, "Id", "Name");
            return View();
        }

        // POST: SavingRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,IsExpense,IsEmergency,Value,SavingsPocketId")] SavingRecord savingRecord)
        {
            if (ModelState.IsValid)
            {
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
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Description,IsExpense,IsEmergency,Value,SavingsPocketId")] SavingRecord savingRecord)
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
        public async Task<IActionResult> CreatePockets([Bind("Id,Name,IsActive,ChallengeValue,Total")] SavingsPocket savingsPocket)
        {
            if (ModelState.IsValid)
            {
                savingsPocket.Id = Guid.NewGuid();
                savingsPocket = await _httpHelper.Post<SavingsPocket>($"{Constants.SAVINGS_POCKETS_URI}", savingsPocket);
                return RedirectToAction(nameof(IndexPockets));
            }
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
        public async Task<IActionResult> EditPockets(Guid id, [Bind("Id,Name,IsActive,ChallengeValue,Total,UserId")] SavingsPocket savingsPocket)
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
    }
}
