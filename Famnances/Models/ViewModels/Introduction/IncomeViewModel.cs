using Famnances.DataCore.Entities;
using Famnances.Helpers;
using Famnances.Resources.ViewModels.Introduction;
using System.ComponentModel.DataAnnotations;

namespace Famnances.Models.ViewModels.Introduction
{
    public class IncomeViewModel
    {
        public Income NewIncome { get; set; }

        public Guid PeriodId { get; set; }
        public string Period { get; set; }

        public decimal Total { get; set; }

        public List<Income> Incomes { get; set; }
        public class Income
        {
            public Income(FixedIncome fixedIncome)
            {
                Description = fixedIncome.Description;
                FirstPayDate = fixedIncome.FirstPayDate;
                PayablePeriodId = fixedIncome.PayablePeriodId;
                Value = fixedIncome.Value;
                ValuePeriodId = fixedIncome.ValuePeriodId;
                Type = IncomeType.Fixed;
            }

            [Display(Name = nameof(IncomeLabels.Description), ResourceType = typeof(IncomeLabels))]
            public string Description { get; set; }

            [Display(Name = nameof(IncomeLabels.Value), ResourceType = typeof(IncomeLabels))]
            public decimal Value { get; set; }

            [Display(Name = nameof(IncomeLabels.PayablePeriod), ResourceType = typeof(IncomeLabels))]
            public Guid PayablePeriodId { get; set; }

            [Display(Name = nameof(IncomeLabels.ValuePeriod), ResourceType = typeof(IncomeLabels))]
            public Guid ValuePeriodId { get; set; }

            public string Period { get; set; }

            [Display(Name = nameof(IncomeLabels.FirstPayDate), ResourceType = typeof(IncomeLabels))]
            public DateTime FirstPayDate { get; set; }

            [Display(Name = nameof(IncomeLabels.Type), ResourceType = typeof(IncomeLabels))]
            public IncomeType Type { get; set; }

        }
    }


}
