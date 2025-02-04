using Microsoft.AspNetCore.Identity;
using static Tracker.Models.ViewModels.EnumsList;

namespace Tracker.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        public Genders Gender { get; set; }
      
        public decimal Income { get; set; }

        public string? ProfilePicture { get; set; }
    }
}