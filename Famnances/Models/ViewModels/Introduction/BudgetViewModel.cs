using Famnances.DataCore.Entities;

namespace Famnances.Models.ViewModels.Introduction
{
    public class BudgetViewModel
    {
        public Guid PeriodId { get; set; }
        public string Period { get; set; }
        public decimal Total { get; set; }
        public ExpensesBudget Budget { get; set; }
        public List<ExpensesBudget> Budgets { get; set; }
    }
}
