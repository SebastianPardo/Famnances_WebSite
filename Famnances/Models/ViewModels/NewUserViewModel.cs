using System.ComponentModel.DataAnnotations;
using Famnances.Resources.ViewModels.Users;

namespace Famnances.Models.ViewModels
{
    public class NewUserViewModel
    {
        [Required(ErrorMessageResourceName = nameof(CreateLabels.RequiredField), ErrorMessageResourceType = typeof(CreateLabels))]
        [Display(Name = nameof(CreateLabels.FirstName), ResourceType = typeof(CreateLabels))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = nameof(CreateLabels.RequiredField), ErrorMessageResourceType = typeof(CreateLabels))]
        [Display(Name = nameof(CreateLabels.LastName), ResourceType = typeof(CreateLabels))]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceName = nameof(CreateLabels.RequiredField), ErrorMessageResourceType = typeof(CreateLabels))]
        [EmailAddress(ErrorMessageResourceName = nameof(CreateLabels.InvalidEmail), ErrorMessageResourceType = typeof(CreateLabels))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = nameof(CreateLabels.RequiredField), ErrorMessageResourceType = typeof(CreateLabels))]
        [Display(Name = nameof(CreateLabels.Password), ResourceType = typeof(CreateLabels))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = nameof(CreateLabels.RequiredField), ErrorMessageResourceType = typeof(CreateLabels))]
        [Display(Name = nameof(CreateLabels.ConfirmPassword), ResourceType = typeof(CreateLabels))]
        public string ConfirmPassword { get; set; }
    }
}
