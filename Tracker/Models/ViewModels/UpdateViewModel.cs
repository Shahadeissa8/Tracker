using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using static Tracker.Models.ViewModels.EnumsList;

public class UpdateViewModel
{
//public string Id { get; set; }  
    public string Name { get; set; }

    [EmailAddress]
    [MinLength(6, ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    [Display(Name = "Profile Picture")]
    public IFormFile? ProfileImageFile { get; set; }

    public string? ProfileImage { get; set; }
    public Genders Gender { get; set; }
    public string? Mobile { get; set; }

}
