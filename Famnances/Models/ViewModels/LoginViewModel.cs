using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Famnances.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DisplayName("User")]
        [EmailAddress]
        public string Param_1 { get; set; }

        [Required]
        [DisplayName("Password")]
        public string Param_2 { get; set; }

        public string Param_3 { get; set; }
    }
}
