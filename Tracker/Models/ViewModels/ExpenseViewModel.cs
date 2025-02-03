using static Tracker.Models.ViewModels.EnumsList;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Tracker.Models.ViewModels
{
    public class ExpenseViewModel
    {
        public Guid ExpenseId { get; set; }

        [Required(ErrorMessage = "Enter The Name")]
        public string ExpenseName { get; set; }
        [Required(ErrorMessage = "Enter The Amount")]
        public decimal ExpenseAmount { get; set; }
        [Required(ErrorMessage = "Enter The Date")]
        public DateTime ExpenseDate { get; set; }

        [Required(ErrorMessage = "Choose a currency")]
        public Currencies Curency { get; set; }
        public bool Recurring { get; set; }
        public string? ExpenseDescription { get; set; }
        [Required(ErrorMessage = "Choose a category")]
        public Category Categories { get; set; }
    }
}
