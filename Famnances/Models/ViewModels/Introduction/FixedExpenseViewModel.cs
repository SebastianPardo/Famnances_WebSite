using Famnances.DataCore.Entities;

namespace Famnances.Models.ViewModels.Introduction
{
    public class FixedExpenseViewModel
    {
        public Guid PeriodId { get; set; }
        public string Period { get; set; }
        public decimal Total { get; set; }
        public FixedExpense Expense { get; set; }
        public List<FixedExpense> Expenses { get; set; }
    }
}
