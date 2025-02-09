using static Tracker.Models.ViewModels.EnumsList;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Tracker.Models.ViewModels
{
    public class ExpenseViewModel
    {
        public Guid ExpenseId { get; set; }

        [Required(ErrorMessage = "Enter The Name")]
        [DisplayName("Expeness Name")]
        public string ExpenseName { get; set; }
        [Required(ErrorMessage = "Enter The Amount")]
        [DisplayName("Expeness Amount")]
        public decimal ExpenseAmount { get; set; }
        [Required(ErrorMessage = "Enter The Date")]
        [DisplayName("Expeness Date")]
        public DateTime ExpenseDate { get; set; }

        [Required(ErrorMessage = "Choose a currency")]
        public Currencies Curency { get; set; }
        public bool Recurring { get; set; }

        [DisplayName("Expeness Description")]
        public string? ExpenseDescription { get; set; }
        [Required(ErrorMessage = "Choose a category")]
        [DisplayName("Expeness Categories")]
        public Category Categories { get; set; }
        [DisplayName("Expeness Recuring")]
        public Recurrings Recurrin { get; set; }
    }
}
