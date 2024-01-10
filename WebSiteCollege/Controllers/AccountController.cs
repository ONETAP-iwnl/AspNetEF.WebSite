using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebSiteCollege.Identity;
using WebSiteCollege.Models;


namespace WebSiteCollege.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult AdminDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
        public IActionResult StudentDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Teacher")]
        public IActionResult TeacherDashboard()
        {
            return View();
        }

        //[HttpGet]
        //public IActionResult Login()
        //{
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Login);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                    if (result.Succeeded)
                    {
                        // Получите роли пользователя
                        var roles = await _userManager.GetRolesAsync(user);

                        // Перенаправление в зависимости от ролей
                        if (roles.Contains("Administrator"))
                            return RedirectToAction("AdminDashboard", "Account");

                        if (roles.Contains("Student"))
                            return RedirectToAction("StudentDashboard", "Account");

                        if (roles.Contains("Teacher"))
                            return RedirectToAction("TeacherDashboard", "Account");
                    }
                }

                ModelState.AddModelError(string.Empty, "аккаунт не найден");
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync().Wait();
            return RedirectToAction("Index", "Home");
        }
    }
}

