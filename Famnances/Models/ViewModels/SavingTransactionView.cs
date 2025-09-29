using Famnances.DataCore.Entities;

namespace Famnances.Models.ViewModels
{
    public class SavingTransactionView
    {
        public SavingRecord SavingTransaction { get; set; }
        public string? SavingSource { get; set; }
    }
}
