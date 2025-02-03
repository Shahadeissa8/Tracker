using static Tracker.Models.ViewModels.EnumsList;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Tracker.Models.ViewModels
{
    public class ExpenseViewModel
    {
        public Guid ExpenseId { get; set; }

        [Required(ErrorMessage = "Enter The Amount")]
        public decimal ExpenseAmount { get; set; }
        public DateTime ExpenseDate { get; set; }

        [Required(ErrorMessage = "Choose a currency")]
        public Currencies Curency { get; set; }
        public bool? Recurring { get; set; }
        [ForeignKey("ApplicationUser")]
        public int UserId { get; set; }
        [ForeignKey("Budget")]
        public int BudgetId { get; set; }

    }
}
