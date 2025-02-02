using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tracker.Models.ViewModels;
using Tracker.Models;
using Microsoft.EntityFrameworkCore;

namespace Tracker.Controllers
{
   // [Authorize]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> userManager;

        private SignInManager<ApplicationUser> signInManager;
        private readonly IWebHostEnvironment webHostEnvironment;
        public AccountController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager, IWebHostEnvironment webHostEnvironment)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            this.webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model);
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = model.Name,
                    Email = model.Email,
                    PhoneNumber = model.Mobile,
                    Gender= model.Gender,
                    ProfilePicture = uniqueFileName
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                    await signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
            return View(model);
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(userName: model.Email, model.Password, false, false);
                if (result.Succeeded)
                {
                    var r = await userManager.GetUserAsync(User);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid user or password");
                return View(model);
            }
            return View();
        }
        private string UploadedFile(RegisterViewModel model)
        {
            string uniqueFileName = null;

            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        //public async Task<IActionResult> Profile(string searchString, DateTime? startDate, DateTime? endDate, decimal? minAmount, decimal? maxAmount)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    var transactions = _context.Transactions.Include(t => t.Receiver).Where(t => t.UserId == user.Id);

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        transactions = transactions.Where(t => t.Amount.ToString().Contains(searchString) || t.Date.ToString("g").Contains(searchString));
        //    }

        //    if (startDate.HasValue)
        //    {
        //        transactions = transactions.Where(t => t.Date >= startDate.Value);
        //    }

        //    if (endDate.HasValue)
        //    {
        //        transactions = transactions.Where(t => t.Date <= endDate.Value);
        //    }

        //    if (minAmount.HasValue)
        //    {
        //        transactions = transactions.Where(t => t.Amount >= minAmount.Value);
        //    }

        //    if (maxAmount.HasValue)
        //    {
        //        transactions = transactions.Where(t => t.Amount <= maxAmount.Value);
        //    }

        //    var filteredTransactions = await transactions.ToListAsync();

        //    return View(filteredTransactions);
        //}
    }
}
