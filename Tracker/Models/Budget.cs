using System.ComponentModel.DataAnnotations.Schema;
using static Tracker.Models.ViewModels.EnumsList;

namespace Tracker.Models
{
    public class Budget
    {
        public Guid BudgetId { get; set; }
        public decimal RemainingAmount { get; set; }
        public Category Categories { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }  
        public decimal Amount { get; set; }
    }
}
/////jkjjkj