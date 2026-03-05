using Famnances.DataCore.Entities;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Famnances.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Famnances.ViewComponents
{
    public class OverspentViewComponent : ViewComponent
    {
        IHttpHelper _httpHelper;
        public OverspentViewComponent(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }
        public async Task<IViewComponentResult> InvokeAsync(OverspentViewModel model)
        {
            ViewBag.SavingPockets = new SelectList(await _httpHelper.Get<List<SavingsPocket>>(Constants.SAVINGS_POCKETS_URI), "Id", "Name");
            ViewBag.ExpensesBudgets = new SelectList(await _httpHelper.Get<List<ExpensesBudget>>(Constants.BUDGETS_URI), "Id", "Name");

            return View(new OutflowViewModel { OverSpent = model });
        }
    }
}
