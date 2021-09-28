using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;
using RemaSoftware.Models.LoginViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemaSoftware.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<MyUser> _signInManager;
        private readonly UserManager<MyUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IWebHostEnvironment _hostEnvironment;
        public LoginViewModel SignUp_Model { get; set; }


        public LoginController(UserManager<MyUser> userManager, SignInManager<MyUser> signInManager, ApplicationDbContext applicationDbContext, IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this._applicationDbContext = applicationDbContext;
            this._hostEnvironment = hostEnvironment;

        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            LoginViewModel viewModel = new LoginViewModel();

            return View(viewModel);
        }

        [HttpPost]                                 
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                MyUser user;

                user = await _userManager.FindByEmailAsync(model.Input.Email);

                if (user == null)
                {
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Input.Password, true, lockoutOnFailure: false);

                if (result.Succeeded)
                {

                    return RedirectToAction("Index", "Home");

                }

            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            ForgotPasswordViewModel viewModel = new ForgotPasswordViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                MyUser user;

                user = await _userManager.FindByEmailAsync(model.ForgotModel.Email);

                if (user == null)
                {
                    return View(model);
                }

            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Login");

        }
    }
}
