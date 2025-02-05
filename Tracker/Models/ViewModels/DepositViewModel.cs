using System.ComponentModel.DataAnnotations;

namespace Tracker.Models.ViewModels
{
    public class DepositViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Enter The Amount you want to deposit")]
        public decimal Amount { get; set; }
    }
}
