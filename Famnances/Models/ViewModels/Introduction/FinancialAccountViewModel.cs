using Famnances.DataCore.Entities;

namespace Famnances.Models.ViewModels.Introduction
{
    public class FinancialAccountViewModel
    {
        public FinancialAccount financialAccount { get; set; }
        public List<FinancialAccount> financialAccounts { get; set; }
    }
}
