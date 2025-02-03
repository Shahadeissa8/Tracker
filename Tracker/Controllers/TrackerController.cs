using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using Microsoft.EntityFrameworkCore;
using Tracker.Data;
using Tracker.Models;
using Tracker.Models.ViewModels;
using System.Security.Claims;

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
        public IActionResult AddExpense(ExpenseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = userManager.GetUserId(User);  

                if (userId == null)
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
                    UserId = userId  
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

            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var latestExpenses =await db.Expenses.Where(e=>e.UserId == userId).OrderByDescending(Exp => Exp.ExpenseDate).ToListAsync();
            return View(latestExpenses);
        }
        
    }
}
