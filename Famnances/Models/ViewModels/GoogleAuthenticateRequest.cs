namespace Famnances.Models.ViewModels;

using System.ComponentModel.DataAnnotations;

public class GoogleAuthenticateRequest
{
    [Required]
    public string Param_1 { get; set; }

    [Required]
    public string Param_2 { get; set; }
}