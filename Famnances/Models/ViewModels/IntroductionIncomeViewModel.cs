namespace Famnances.Models.ViewModels
{
    public class IntroductionIncomeViewModel
    {
        public Income NewIncome { get; set; }

        public Guid PeriodId { get; set; }
        public string Period { get; set; }

        public decimal Total { get; set; }

        public List<Income> Incomes { get; set; }
        public class Income
        {
            public string Description { get; set; }

            public decimal Value { get; set; }

            public Guid PeriodId { get; set; }

            public string Period { get; set; }

            public DateTime FirstPayDate { get; set; }

            public string Type { get; set; }

        }
    }


}
