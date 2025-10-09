using Famnances.DataCore.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Famnances.Models.ViewModels
{
    public class FixedIncomeViewModel
    {
        public FixedIncome FixedIncome { get; set; }
        public List<Guid> SelectedIncomeDiscountIds { get; set; }
    }
}
