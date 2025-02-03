using static Tracker.Models.ViewModels.EnumsList;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tracker.Models
{
    public class Expense
    {
        public Guid ExpenseId { get; set; }

        [Required(ErrorMessage = "Enter The Amount")]
        public decimal ExpenseAmount { get; set; }
        public DateTime ExpenseDate { get; set; }
        [Required(ErrorMessage = "Choose a currency")]
        public Currencies Curency { get; set; }
        public bool? Recurring { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }//supposed to be string right? because in identity user it is a string
        [ForeignKey("Budget")]
        public int BudgetId { get; set; }
        public string ExpenseName { get; set; }
        public string? ExpenseDescription { get; set; }

    }
}
