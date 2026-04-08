using Famnances.Resources.ViewModels.Introduction;
using System.ComponentModel.DataAnnotations;

namespace Famnances.Models.ViewModels.Introduction
{
    public class SavingsViewModel
    {
        public Guid PeriodId { get; set; }
        public string Period { get; set; }
        public decimal Total { get; set; }
        public SavingPocket Pocket { get; set; }
        public List<SavingPocket> Pockets { get; set; }

        public class SavingPocket
        {
            [Display(Name = nameof(SavingsLabels.FrecuentDeposits), ResourceType = typeof(SavingsLabels))]
            public bool FrecuentDeposits { get; set; }

            [Display(Name = nameof(SavingsLabels.Name), ResourceType = typeof(SavingsLabels))]
            public string Name { get; set; }

            [Display(Name = nameof(SavingsLabels.FrecuentValue), ResourceType = typeof(SavingsLabels))]
            public decimal? FrecuentValue { get; set; }

            [Display(Name = nameof(SavingsLabels.ChallengeValue), ResourceType = typeof(SavingsLabels))]
            public decimal? ChallengeValue { get; set; }

            [DataType(DataType.Currency)]
            [Display(Name = nameof(SavingsLabels.Total), ResourceType = typeof(SavingsLabels))]
            public decimal Total { get; set; }
        }
    }
}
