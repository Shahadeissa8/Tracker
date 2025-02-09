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
using Microsoft.AspNetCore.Components.Forms;

namespace Tracker.Controllers
{
    public class TrackerController : Controller
    {
        private DateTime Edate;
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
                Expense expenses = new Expense()
                {
                    ExpenseName = model.ExpenseName,
                    ExpenseAmount = model.ExpenseAmount,
                    ExpenseDate = model.ExpenseDate,
                    Curency = model.Curency,
                    Categories = model.Categories,
                    Recurring = model.Recurring,
                    UserId = userId,
                    ExpenseDescription = model.ExpenseDescription
                };
                db.Expenses.Add(expenses);
                db.SaveChanges();

                switch (model.Recurrin.ToString())
                {
                    case "Daily":
                        Edate = model.ExpenseDate.AddDays(1);
                        break;
                    case "Weekly":
                        Edate = model.ExpenseDate.AddDays(7);
                        break;
                    case "Monthly":
                        Edate = model.ExpenseDate.AddMonths(1);
                        break;
                    case "yearly":
                        Edate = model.ExpenseDate.AddYears(1);
                        break;
                    default:
                        Edate = model.ExpenseDate;
                        break;
                }

                if (model.Recurring)
                {
                    Expense recurringExpense = new Expense()
                    {

                        ExpenseName = model.ExpenseName,
                        ExpenseAmount = model.ExpenseAmount,
                        ExpenseDate = Edate, 
                        Curency = model.Curency,
                        Categories = model.Categories,
                        Recurring = model.Recurring,
                        UserId = userId,
                        ExpenseDescription = model.ExpenseDescription
                    };

                    db.Expenses.Add(recurringExpense);
                    db.SaveChanges();
                }
                if (model.Recurring)
                {
                    return RedirectToAction("ViewExpenses", "Tracker"); 
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
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

            var FindExpense = await db.Expenses.Where(Exp => Exp.UserId == userId).ToListAsync();
            if (model.Amount < 0)
            {
                ModelState.AddModelError(string.Empty, "There’s no such expense with an invalid amount.");
                return View(new SearchViewModel { ExpensesList = new List<Expense>() });
            }
            else if (model.Amount > 0)
            {
                FindExpense = FindExpense.Where(Exp => Exp.ExpenseAmount == model.Amount).ToList();
            }

            if (!string.IsNullOrEmpty(model.SearchString))
            {
                var matchedExpenses = FindExpense.Where(Exp => Exp.ExpenseName.Contains(model.SearchString)).ToList();

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
            };
            var remainingBudget = db.Budget.FirstOrDefault(b => b.UserId == userId)?.RemainingAmount ?? 0;

            model.ExpensesList = search.ExpensesList;

            ViewData["RemainingBudget"] = remainingBudget;

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

            var transactions = db.Expenses.Where(e => e.UserId == user.Id).AsQueryable();

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
        public async Task<IActionResult> Delete(Guid expenseId)
        {
            var expense = await db.Expenses.FindAsync(expenseId);
            if (expense == null)
            {

                return NotFound();
            }
            var user = await userManager.GetUserAsync(User);
            if (expense.UserId != user.Id)
            {

                return Unauthorized();
            }
            db.Expenses.Remove(expense);
            await db.SaveChangesAsync();
            return RedirectToAction("ViewExpenses");
        }

    }
}