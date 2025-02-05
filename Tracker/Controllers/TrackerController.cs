using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using Microsoft.EntityFrameworkCore;
using Tracker.Data;
using Tracker.Models;
using Tracker.Models.ViewModels;
using static Tracker.Models.ViewModels.EnumsList;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Tracker.Controllers
{
    public class TrackerController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;
        public TrackerController(ApplicationDbContext _db, UserManager<ApplicationUser> _userManager)
        {
            db = _db;
            userManager = _userManager;
        }
        [HttpGet]
        public IActionResult AddExpense()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddExpense(ExpenseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = userManager.GetUserId(User);
                if (userId == null || string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                Expense expense = new Expense()
                {
                    ExpenseName = model.ExpenseName,
                    ExpenseAmount = model.ExpenseAmount,
                    ExpenseDate = model.ExpenseDate,
                    Curency = model.Curency,
                    Categories = model.Categories,
                    UserId = userId,
                    ExpenseDes = model.ExpenseDescription,
                    Recurrin = model.Recurrin
                };

                db.Expenses.Add(expense);
                db.SaveChanges();

                if (model.Recurring)
                {
                    ScheduleNextOccurrence(expense);
                }

                return RedirectToAction("ViewExpenses");
            }
            return View(model);
        }

        public async Task<IActionResult> ViewExpenses(SearchViewModel model)
        {
            var userId = userManager.GetUserId(User);
            if (userId == null || string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var FindExpense = await db.Expenses.Where(Exp => Exp.UserId == userId).ToListAsync(); //change my transaction to the view expenses from mohammad
            if (model.Amount < 0)  // Allowing 0 to mean "no filter"
            {
                ModelState.AddModelError(string.Empty, "There’s no such expense with an invalid amount.");
                return View(new SearchViewModel { ExpensesList = new List<Expense>() }); //again change this to the name of Mohammads view as well
            }//IMPORTANT NOTE: the view is in my desktop, i will add it later after we finish correcting everything in this action and create the view (adding what mohammad adds)
            else if (model.Amount > 0)
            {
                FindExpense = FindExpense.Where(Exp => Exp.ExpenseAmount == model.Amount).ToList();
            }

            if (!string.IsNullOrEmpty(model.SearchString))
            {
                var matchedExpenses = FindExpense
                    .Where(Exp => Exp.ExpenseName.Contains(model.SearchString) || Exp.ExpenseDes.Contains(model.SearchString))
                    .ToList();

                if (matchedExpenses.Any())
                {
                    FindExpense = matchedExpenses;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "There’s no such expense matching the search criteria.");
                }
            }

            if (model.FromDate == DateTime.MinValue || model.ToDate == DateTime.MinValue)
            {
                ModelState.AddModelError(string.Empty, "There’s no such expense with this date.");
            }
            else if (model.FromDate != DateTime.MinValue && model.ToDate != DateTime.MinValue)
            {
                FindExpense = FindExpense.Where(Exp => Exp.ExpenseDate.Date >= model.FromDate && Exp.ExpenseDate.Date <= model.ToDate).ToList();
            }

            var search = new SearchViewModel()
            {
                ExpensesList = FindExpense.OrderByDescending(Exp => Exp.ExpenseDate).ToList()
            };//to write everything in the view in a descending based on date
            return View(search);


        }
        public async Task<IActionResult> LatestTransactions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var latestTransactions = await db.Expenses
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();

            return View();
        }
        public async Task<IActionResult> Filter(string searchString, DateTime? startDate, DateTime? endDate, decimal? minAmount, decimal? maxAmount, Currencies? currency)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return View();
            }

            // Start with all transactions for the current user
            var transactions = db.Expenses.Where(e => e.UserId == user.Id).AsQueryable();

            // Apply filters if they are provided
            if (!string.IsNullOrEmpty(searchString))
            {
                transactions = transactions.Where(t => t.ExpenseAmount.ToString().Contains(searchString) || t.ExpenseDate.ToString("g").Contains(searchString));
            }
            if (startDate.HasValue)
            {
                transactions = transactions.Where(e => e.ExpenseDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                transactions = transactions.Where(e => e.ExpenseDate <= endDate.Value);
            }

            if (minAmount.HasValue)
            {
                transactions = transactions.Where(e => e.ExpenseAmount >= minAmount.Value);
            }

            if (maxAmount.HasValue)
            {
                transactions = transactions.Where(e => e.ExpenseAmount <= maxAmount.Value);
            }

            if (currency.HasValue)  // Filter by currency if provided
            {
                transactions = transactions.Where(e => e.Curency == currency.Value); // Use the correct currency field
            }

            // Fetch filtered transactions
            var filteredTransactions = await transactions.OrderByDescending(e => e.ExpenseDate).ToListAsync();

            return View(filteredTransactions);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF protection
        public async Task<IActionResult> Delete(int id)
        {
            var expense = await db.Expenses.FindAsync(id);

            if (expense == null)
            {
                return NotFound();
            }

            // Ensure that the user is the one who created the expense (security check)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (expense.UserId != userId)
            {
                return Unauthorized();
            }

            db.Expenses.Remove(expense);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(LatestTransactions));
        }
   
    }
}

