using System.ComponentModel.DataAnnotations;
using Famnances.Resources.ViewModels.Savings;

namespace Famnances.Models.ViewModels
{
    public class PocketsTransferModel
    {
        [Required(ErrorMessageResourceName = nameof(SavingsLabels.RequiredField), ErrorMessageResourceType = typeof(SavingsLabels))]
        [Display(Name = nameof(SavingsLabels.From), ResourceType = typeof(SavingsLabels))]
        public Guid FromPocket { get; set; }

        [Required(ErrorMessageResourceName = nameof(SavingsLabels.RequiredField), ErrorMessageResourceType = typeof(SavingsLabels))]
        [Display(Name = nameof(SavingsLabels.To), ResourceType = typeof(SavingsLabels))]
        public Guid ToPocket { get; set; }

        [Required(ErrorMessageResourceName = nameof(SavingsLabels.RequiredField), ErrorMessageResourceType = typeof(SavingsLabels))]
        [Display(Name = nameof(SavingsLabels.Value), ResourceType = typeof(SavingsLabels))]
        public decimal Value { get; set; }
    }
}
