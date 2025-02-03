using static Tracker.Models.ViewModels.EnumsList;

namespace Tracker.Models.ViewModels
{
    public class SearchViewModel
    {
        public List<Expense>? ExpensesList { get; set; }//change list of "My Transactions" to what Mohammad named it
        public decimal Amount { get; set; } 
        public DateTime FromDate { get; set; } //for filtering
        public DateTime ToDate { get; set; }
        public string ExpenseName { get; set; }
        public string? ExpenseDescription { get; set; }
        public string? SearchString { get; set; }
        //public string? Type { get; set; }
    }
}
