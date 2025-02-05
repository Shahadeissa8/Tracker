
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tracker.Models.ViewModels;
using Tracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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
            ViewData["CurrentPage"] = "Register";
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model.ProfileImage);
                ApplicationUser user = new ApplicationUser()
                {
                    Name = model.Name,
                    UserName = model.Email, // REQUIRED
                    Email = model.Email,
                    PhoneNumber = model.Mobile,
                    Gender = model.Gender,
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
                    Console.WriteLine(error.Description); // Debugging
                }
                return View(model);
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
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);

            if (!User.Identity.IsAuthenticated || !signInManager.IsSignedIn(User))
            {
                RedirectToAction("Login");
            }
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {


            // var user = await userManager.FindByIdAsync(id);
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
                Gender = user.Gender,
                ProfileImage = user.ProfilePicture
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

                if (model.ProfileImageFile != null)
                {
                    string uniqueFile = UploadedFile(model.ProfileImageFile);
                    user.ProfilePicture = uniqueFile;
                }

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Profile");
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

        //private string UploadedFile(IFormFile file)
        //{
        //    string uniqueFileName = null;

        //    if (file != null)
        //    {
        //        string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
        //        uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        //        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            file.CopyTo(fileStream);
        //        }
        //    }
        //    return uniqueFileName;
        //}
        private string UploadedFile(IFormFile file)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
