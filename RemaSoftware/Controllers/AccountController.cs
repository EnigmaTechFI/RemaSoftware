using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;
using RemaSoftware.Models.LoginViewModel;
using System.Threading.Tasks;

namespace RemaSoftware.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<MyUser> _signInManager;
        private readonly UserManager<MyUser> _userManager;
        public LoginViewModel SignUp_Model { get; set; }


        public AccountController(UserManager<MyUser> userManager, SignInManager<MyUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
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

                var result = await _signInManager.PasswordSignInAsync(user, model.Input.Password, model.Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {

                    return RedirectToAction("Index", "Home");

                }

            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
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
            return RedirectToAction("Login", "Account");

        }
    }
}
