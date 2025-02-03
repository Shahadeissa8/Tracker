using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using Microsoft.EntityFrameworkCore;
using Tracker.Data;
using Tracker.Models;
using Tracker.Models.ViewModels;

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
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddExpense()
        {
          //  ViewBag.Depts = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            return View();
        }
        [HttpPost]
        public IActionResult AddExpense(Expense model)
        {
            if (ModelState.IsValid)
            {
                Expense expenses = new Expense()
                {
                    ExpenseAmount = model.ExpenseAmount,
                    ExpenseDate = model.ExpenseDate,
                    Curency = model.Curency,
                    Recurring = model.Recurring,
                };
                db.Expenses.Add(expenses);
                db.SaveChanges();
                return RedirectToAction("Home", "Index");
            }
       //     ViewBag.Depts = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            return View(model);
        }

        //the search action
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
                    .Where(Exp => Exp.ExpenseName.Contains(model.SearchString) || Exp.ExpenseDescription.Contains(model.SearchString))
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
        public IActionResult FilterByCategory(Budget model)
        {
            var FindExpense =  db.Expenses.Where(Exp => Exp.UserId == UserId); //change my transaction to the view expenses from mohammad
            if (model.Categories != null && !model.Categories.Equals("All"))
            {
                FindExpense = FindExpense.Where(Exp => Exp.Category.ToString().Equals(model.Categories)).ToList();

            }
            return View();
        }
    }
}
