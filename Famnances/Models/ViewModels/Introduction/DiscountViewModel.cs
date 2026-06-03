using Famnances.DataCore.Entities;
using Famnances.Resources.ViewModels.Introduction;
using System.ComponentModel.DataAnnotations;

namespace Famnances.Models.ViewModels.Introduction
{
    public class DiscountViewModel
    {
        public Guid PeriodId { get; set; }
        public string Period { get; set; }
        public decimal Total { get; set; }
        public Discount IncomeDiscount { get; set; }
        public List<Discount> IncomeDiscounts { get; set; }

        public class Discount
        {
            public Discount() { }
            public Discount(IncomeDiscount incomeDiscount)
            {
                Description = incomeDiscount.Description;
                Value = incomeDiscount.Value;
                IncomeIds = incomeDiscount.FixedIncomeByDiscount.Select(e => e.FixedIncomeId).ToList();
                ByPayablePeriod = incomeDiscount.FixedIncomeByDiscount.First().ByPayablePeriod;
                IsPercentage = incomeDiscount.IsPercentage;
                IsPrediscount = incomeDiscount.IsPrediscount;
                IsTax = incomeDiscount.IsTax;
            }

            [Display(Name = nameof(DiscountsLabels.Description), ResourceType = typeof(DiscountsLabels))]
            public string Description { get; set; }

            [Display(Name = nameof(DiscountsLabels.Value), ResourceType = typeof(DiscountsLabels))]
            [DataType(DataType.Currency)]
            public decimal Value { get; set; }

            [Display(Name = nameof(DiscountsLabels.FixedIncomes), ResourceType = typeof(DiscountsLabels))]
            public List<Guid> IncomeIds { get; set; }

            [Display(Name = nameof(DiscountsLabels.ByPayablePeriod), ResourceType = typeof(DiscountsLabels))]
            public bool ByPayablePeriod { get; set; }

            [Display(Name = nameof(DiscountsLabels.IsPercentage), ResourceType = typeof(DiscountsLabels))]
            public bool IsPercentage { get; set; }

            [Display(Name = nameof(DiscountsLabels.IsPrediscount), ResourceType = typeof(DiscountsLabels))]
            public bool IsPrediscount { get; set; }

            [Display(Name = nameof(DiscountsLabels.IsTax), ResourceType = typeof(DiscountsLabels))]
            public bool IsTax { get; set; }
        }
    }
}
