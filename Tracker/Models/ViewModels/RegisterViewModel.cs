using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using static Tracker.Models.ViewModels.EnumsList;

namespace Tracker.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Enter Your Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Enter Your Email")]
        [EmailAddress]
        [MinLength(6, ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Confirm Your Email")]
        [EmailAddress]
        [Compare("Email", ErrorMessage = "Email not match")]
        public string ConfirmEmail { get; set; }
        [Required(ErrorMessage = "Enter Your Password")]
        [DataType(DataType.Password)]
        [MinLength(3, ErrorMessage = "Invalid password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Your Paswword")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password not match")]
        public string ConfirmPassword { get; set; }
        public Genders Gender { get; set; }
        public string? Mobile { get; set; }
        [Display(Name = "Profile Picture")]
        public IFormFile? ProfileImage { get; set; }
    }
}
