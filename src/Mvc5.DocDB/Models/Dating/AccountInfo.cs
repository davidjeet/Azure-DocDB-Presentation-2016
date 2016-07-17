namespace Mvc5.DocDB.Models.Dating
{
    using System.ComponentModel.DataAnnotations;

    public class AccountInfo
    {
        [Required(ErrorMessage = "Must have  a user name.")]  
        public string Username { get; set; }

        [Required(ErrorMessage = "Must have  a password.")]
        public string Password { get; set; }
    }
}