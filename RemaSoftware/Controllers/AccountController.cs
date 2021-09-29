using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RemaSoftware.ContextModels;
using RemaSoftware.Data;
using RemaSoftware.Models.LoginViewModel;
using System.Threading.Tasks;
using EmailService;

namespace RemaSoftware.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<MyUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly UserManager<MyUser> _userManager;
        public LoginViewModel SignUp_Model { get; set; }


        public AccountController(UserManager<MyUser> userManager, SignInManager<MyUser> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
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
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (!ModelState.IsValid)
                return null; // todo sistema per mostrate toast errore
 
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return null; // // todo sistema per mostrate toast errore
            
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var returnUrl = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);
 
            bool emailResponse = _emailService.SendEmailForPasswordReset(returnUrl, email);
 
            if (emailResponse)
                return RedirectToAction("Login"); // todo tast con avviso che è stata mandata la mail
            else
            {
                // log email failed 
            }
            return View(email);
            
        }
        
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");

        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return null; // todo
        }
    }
}
