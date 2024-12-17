namespace Blazor.Web.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string Source { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int PubId { get; set; }
        public DateTime? HireDate { get; set; }

        public Role Role { get; set; }
        public string ConfirmPassword { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
