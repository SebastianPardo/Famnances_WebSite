using System.Runtime.InteropServices;

namespace Famnances.Models.ViewModels
{
    public class HomeViewModel
    {
        public bool ToBeClosed { get; set; }
        public string PeriodName { get; set; }
        public decimal PeriodBudget { get; set; }
        public decimal PeriodSpent { get; set; }
        public decimal PeriodLeft => PeriodBudget - PeriodSpent;
        public decimal PeriodPercentage => PeriodBudget == 0 ? 0 : (int)((PeriodSpent / PeriodBudget) * 100);
        public decimal Chequing { get; set; }
        public decimal Savings { get; set; }
        public decimal PeriodSavingsSpent { get; set; }
        public decimal PeriodSavings => Savings + PeriodSavingsSpent;
        public decimal PeriodSavingsLeft => PeriodSavings - PeriodSavingsSpent;
        public decimal PeriodSavingsPercentage => Savings == 0 ? 0 : (int)((PeriodSavingsSpent / Savings) * 100);
        public decimal HomeSavings { get; set; }

        public List<RoommateModel> Roommates { get; set; }
    }

    public class RoommateModel
    {
        public string Name { get; set; }
        public bool IsCurrentUser { get; set; }

        public List<SummaryFixedExpensesModel> SummaryFixedExpenses { get; set; }
        public decimal TotalFixedExpenses => SummaryFixedExpenses == null ? 0 : SummaryFixedExpenses.Sum(e => e.Value);
        public decimal PaidFixedExpenses => SummaryFixedExpenses == null ? 0 : SummaryFixedExpenses.Where(e => e.WasPaid).Sum(e => e.Value);
        public decimal PercentajePaidFixedExpenses => TotalFixedExpenses == 0 ? 0 : (int)((PaidFixedExpenses / TotalFixedExpenses) * 100);


        public List<SummaryBudgetModel> SummaryBudgets { get; set; }
        public decimal TotalBudget => SummaryBudgets == null ? 0 : SummaryBudgets.Sum(e => e.Budget);
        public decimal BudgetSpent => SummaryBudgets == null ? 0 : SummaryBudgets.Sum(e => e.Spent);
        public decimal PercentajeBudgetSpent => TotalBudget == 0 ? 0 : (int)((BudgetSpent / TotalBudget) * 100);

        public List<SummaryPocketModel> SummaryPockets { get; set; }
        public decimal TotalSavings => SummaryPockets == null ? 0 : SummaryPockets.Sum(e => e.InitialValue);
        public decimal SavingsSpent => SummaryPockets == null ? 0 : SummaryPockets.Sum(e => e.Spent);
        public int PercentajeSavingSpent => SummaryPockets == null ? 0 : (int)((SavingsSpent / TotalSavings) * 100);
    }

    public class SummaryFixedExpensesModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public bool WasPaid { get; set; }
    }

    public class SummaryBudgetModel
    {
        public Guid Id { get; set; }
        public Guid BudgetPeriodBalanceId { get; set; }
        public string Name { get; set; }
        public decimal Budget { get; set; }
        public decimal Spent { get; set; }
        public decimal Left => Budget - Spent;
        public int PercentajeSpent => Budget == 0 ? 0 : (int)((Spent / Budget) * 100);
    }

    public class SummaryPocketModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public decimal Spent { get; set; }
        public decimal InitialValue => Value + Spent;
        public decimal Left => InitialValue - Spent;
        public int PercentajeSpent => Value == 0 ? 0 : (int)((Spent / InitialValue) * 100);
        public decimal Goal { get; set; }
        public int PercentajeSaved => Goal == 0 ? 0 : (int)((Value / Goal) * 100);
    }
}
