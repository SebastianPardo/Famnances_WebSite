namespace Famnances.Models.ViewModels
{
    public class LoginResponseViewModel
    {
        public Guid AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string Language {  get; set; }
        public bool IsFirstLogin { get; set; }
    }
}
