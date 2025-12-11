using Famnances.DataCore.Entities;
using System.ComponentModel;

namespace Famnances.Models.ViewModels
{
    public class SavingTransactionViewModel
    {
        public SavingRecord SavingTransaction { get; set; }
        public string? SavingSource { get; set; }

        [DisplayName("Transfer to checking?")]
        public bool TranferToChequing { get; set; }

        public List<FixedSaving?>? FixedSavings { get; set; }
    }
}
