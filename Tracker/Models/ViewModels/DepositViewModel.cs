using System.ComponentModel.DataAnnotations;

namespace Tracker.Models.ViewModels
{
    public class DepositViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Enter the amount you want to deposit")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }
    }
}
