namespace Famnances.Models.ViewModels
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            PeriodLeft = PeriodBudget - PeriodSpent;
            PeriodSavingsLeft = Savings - PeriodSavingsSpent;
        }

        public bool ToBeClosed { get; set; }
        public string PeriodName { get; set; }
        public decimal PeriodBudget { get; set; }
        public decimal PeriodSpent { get; set; }
        public decimal PeriodLeft { get; set; }
        public decimal Chequing { get; set; }
        public decimal Savings { get; set; }
        public decimal PeriodSavingsSpent { get; set; }
        public decimal PeriodSavingsLeft { get; set; }
        public decimal HomeSavings { get; set; }

        public List<RoommateModel> Roommates { get; set; }
    }

    public class RoommateModel
    {
        public RoommateModel()
        {
            TotalFixedExpenses = SummaryFixedExpenses.Sum(e => e.Value);
            PaidFixedExpenses = SummaryFixedExpenses.Where(e => e.WasPaid).Sum(e => e.Value);

            TotalBudget = SummaryBudgets.Sum(e => e.Budget);
            BudgetSpent = SummaryBudgets.Sum(e => e.Spent);
        }

        public string Name { get; set; }
        public bool IsCurrentUser { get; set; }

        public decimal TotalFixedExpenses { get; set; }
        public decimal PaidFixedExpenses { get; set; }
        public List<SummaryFixedExpensesModel> SummaryFixedExpenses { get; set; }

        public decimal TotalBudget { get; set; }
        public decimal BudgetSpent { get; set; }
        public List<SummaryBudgetModel> SummaryBudgets { get; set; }

        public List<SummaryPocketModel> SummaryPockets { get; set; }
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
        public SummaryBudgetModel()
        {
            PercentajeSpent = Budget == 0 ? 0 : (int)((Spent / Budget) * 100);
            Left = Budget - Spent;
        }
        public Guid Id { get; set; }
        public Guid BudgetPeriodBalanceId { get; set; }
        public string Name { get; set; }
        public decimal Budget { get; set; }
        public decimal Spent { get; set; }
        public decimal Left { get; set; }
        public int PercentajeSpent { get; set; } 
    }

    public class SummaryPocketModel
    {
        public SummaryPocketModel()
        {
            Left = Value - Spent;
            PercentajeSpent = Value == 0 ? 0 : (int)((Spent / Value) * 100);

            PercentajeSaved = Goal == 0 ? 0 : (int)((Value / Goal) * 100);
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public decimal Spent { get; set; }
        public decimal Left { get; set; }
        public int PercentajeSpent { get; set; }
        public decimal Goal { get; set; }
        public decimal PercentajeSaved { get; set; }
    }
}
