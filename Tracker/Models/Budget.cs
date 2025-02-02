using System.ComponentModel.DataAnnotations.Schema;

namespace Tracker.Models
{
    public class Budget
    {
        public Guid BudgetId { get; set; }
        public decimal RemainingAmount { get; set; }

        public string CategoryName { get; set; }
        [ForeignKey("ApplicationUser")]
        public int UserId { get; set; }

       
    }
}
