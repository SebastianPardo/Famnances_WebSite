using System.ComponentModel.DataAnnotations;

namespace Famnances.Models.ViewModels.Introduction
{
    public class SummaryViewModel
    {
        public string Period { get; set; }
        public decimal BudgetByPeriod { get; set; }
        public decimal Total { get; set; }
        public decimal SavingsByPeriod { get; set; }
        public decimal Savings { get; set; }

        [Required]
        [Display(Name = "Suma de tus cuentas + efectivo disponible")]
        public decimal CurrentMoney { get; set; }
    }
}
