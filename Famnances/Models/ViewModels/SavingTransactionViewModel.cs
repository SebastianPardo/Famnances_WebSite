using Famnances.DataCore.Entities;
using Famnances.Resources.ViewModels.Savings;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Famnances.Models.ViewModels
{
    public class SavingTransactionViewModel
    {
        public SavingRecord SavingTransaction { get; set; }

        [Display(Name = nameof(SavingsLabels.SavignsSource), ResourceType = typeof(SavingsLabels))]
        public string? SavingSource { get; set; }

        [Display(Name = nameof(SavingsLabels.ToChecking), ResourceType = typeof(SavingsLabels))]
        public bool TranferToChequing { get; set; }

        public List<FixedSaving?>? FixedSavings { get; set; }
    }
}
