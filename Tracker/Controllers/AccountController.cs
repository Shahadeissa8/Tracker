using Microsoft.AspNetCore.Authorization;
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
                return RedirectToAction("Index", "Home");
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
        [Authorize]
        public async Task<IActionResult> EditProfile(string? id)
        {
            if (id == null)
            {
                return RedirectToAction("Login");
            }

            var user = await userManager.FindByIdAsync(id); 
            if (user == null)
            {
                return RedirectToAction("Login"); 
            }

            UpdateViewModel model = new UpdateViewModel()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };

            return View(model);  
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(UpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);  // If the model is invalid, return the same view with error messages.
            }

            var user = await userManager.FindByIdAsync(model.Id);  // Find the user by ID.
            if (user == null)
            {
                return RedirectToAction("Login");  // If user not found, redirect to login.
            }

            // Update the user data
            user.Name = model.Name;
            user.Email = model.Email;

            // Handle profile picture upload if present
            if (model.ProfileImage != null)
            {
                // Your logic to save the profile image (e.g., save it to the file system or database)
                var fileName = Path.GetFileName(model.ProfileImage.FileName);
                var filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);  // Save the uploaded file
                }

                user.ProfilePicture = fileName;  // Update the user's profile picture
            }

            // Save the updated user to the database
            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile updated successfully.";  // Show success message
                return RedirectToAction("Profile", new { id = user.Id });  // Redirect to profile page (or wherever you need)
            }

            // If there were errors saving the user, display them
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);  // If something went wrong, return the view with the current model
        }


        private string UploadedFile(IFormFile file)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
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
