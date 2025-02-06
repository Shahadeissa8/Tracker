
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tracker.Models.ViewModels;
using Tracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Tracker.Data;
using Microsoft.AspNetCore.Http;

namespace Tracker.Controllers
{
     [Authorize]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;
        private readonly IWebHostEnvironment webHostEnvironment;
        private ApplicationDbContext db;
        public AccountController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager, IWebHostEnvironment webHostEnvironment, ApplicationDbContext _db)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            this.webHostEnvironment = webHostEnvironment;
            db = _db;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewData["CurrentPage"] = "Register";
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
                    Name = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.Mobile,
                    Gender = model.Gender,
                    ProfilePicture = uniqueFileName,
                    Amount = model.Budget 
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Profile", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            ViewData["CurrentPage"] = "Login";
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
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null || !User.Identity.IsAuthenticated || !signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            //HttpContext.Session.SetString("UserBalance", user.Amount!.ToString() ?? "0");


            return View(user);
        }
      
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UpdateViewModel
            {
                Name = user.Name,
                Email = user.Email,
                Mobile = user.PhoneNumber,
                Gender = user.Gender
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(UpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                user.Name = model.Name;
                user.Email = model.Email;
                user.PhoneNumber = model.Mobile;
                user.Gender = model.Gender;

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("profile");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmLogout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private string UploadedFile(RegisterViewModel model)
        {
            string uniqueFileName = null;

            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
