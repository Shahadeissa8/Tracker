using System.ComponentModel.DataAnnotations;

public class UpdateViewModel
{
    [Required(ErrorMessage = "Enter Your Name")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Enter Your Email")]
    [EmailAddress]
    [MinLength(6, ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    [Display(Name = "Profile Picture")]
    public IFormFile? ProfileImage { get; set; }
}
