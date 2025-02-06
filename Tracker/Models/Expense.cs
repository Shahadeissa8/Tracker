using static Tracker.Models.ViewModels.EnumsList;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tracker.Models
{
    public class Expense
    {
        public Guid ExpenseId { get; set; }

        [Required(ErrorMessage = "Enter The Name")]
        public string ExpenseName { get; set; }
        [Required(ErrorMessage = "Enter The Amount")]
        public decimal ExpenseAmount { get; set; }
        public DateTime ExpenseDate { get; set; }
        [Required(ErrorMessage = "Choose a currency")]
        public Currencies Curency { get; set; }
        //public bool? Recurring { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }//supposed to be string right? because in identity user it is a string
        public string? ExpenseDes { get; set; }
        [Required(ErrorMessage = "Choose a category")]
        public Category Categories { get; set; }
        public Recurrings Recurrin { get; set; }
    }
}
