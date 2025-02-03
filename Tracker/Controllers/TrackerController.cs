using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tracker.Data;
using Tracker.Models;
using Tracker.Models.ViewModels;

namespace Tracker.Controllers
{
    public class TrackerController : Controller
    {
        private UserManager<ApplicationUser> userManager;

        private SignInManager<ApplicationUser> signInManager;
        private ApplicationDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;
        public TrackerController(UserManager<ApplicationUser> _userManager, 
            SignInManager<ApplicationUser> _signInManager, 
            IWebHostEnvironment webHostEnvironment, 
            ApplicationDbContext _db)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            db = _db;
            this.webHostEnvironment = webHostEnvironment;
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

    }
}
