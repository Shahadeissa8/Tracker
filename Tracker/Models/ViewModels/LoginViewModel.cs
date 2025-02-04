using System.ComponentModel.DataAnnotations;

namespace Tracker.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Enter Your Email")]
        [EmailAddress]
        [MinLength(6, ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Enter Your Password")]
        [DataType(DataType.Password)]
        [MinLength(3, ErrorMessage = "Invalid password")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }

    }
}
