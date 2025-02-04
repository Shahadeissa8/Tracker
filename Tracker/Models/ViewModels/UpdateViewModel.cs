using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using static Tracker.Models.ViewModels.EnumsList;

public class UpdateViewModel
{
//    [Required(ErrorMessage = "Enter Your Name")]
public string Id { get; set; }  
    public string Name { get; set; }

   // [Required(ErrorMessage = "Enter Your Email")]
    [EmailAddress]
    [MinLength(6, ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    [Display(Name = "Profile Picture")]
    public IFormFile? ProfileImage { get; set; }
    public string Password { get; set; }
    public Genders Gender { get; set; }
    public string? Mobile { get; set; }

}
