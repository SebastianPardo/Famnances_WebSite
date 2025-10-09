namespace Famnances.Models.ViewModels;

using System.ComponentModel.DataAnnotations;

public class ExternalAuthenticateViewModel
{
    [Required]
    public string Param_1 { get; set; }

    [Required]
    public string Param_2 { get; set; }

    [Required]
    public string Param_3 { get; set; }
}