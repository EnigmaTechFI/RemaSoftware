using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;
using RemaSoftware.WebApp.Models.LoginViewModel;
using RemaSoftware.UtilityServices.Interface;
using RemaSoftware.WebApp.Helper;
using RemaSoftware.WebApp.Models.AccountingViewModel;

namespace RemaSoftware.WebApp.Controllers
{
    public class  AccountController : Controller
    {
        private readonly SignInManager<MyUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly UserManager<MyUser> _userManager;
        private readonly INotyfService _notyfToastService;
        private readonly AccountHelper _accountHelper;
        
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public AccountController(UserManager<MyUser> userManager, SignInManager<MyUser> signInManager, IEmailService emailService, INotyfService notyfToastService, AccountHelper accountHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _notyfToastService = notyfToastService;
            _accountHelper = accountHelper;
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
                    _notyfToastService.Error("User not found.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Input.Password, model.Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {

                    var roles = await _userManager.GetRolesAsync(user);
                    if(roles.Contains(Roles.Cliente))
                        return RedirectToAction("Index", "Guest");
                    if(roles.Contains(Roles.Magazzino))
                        return RedirectToAction("StockSummary", "Order");
                    if (roles.Contains(Roles.COQ))
                        return RedirectToAction("QualityControl", "Order");
                    if (roles.Contains(Roles.Dipendente))
                        return RedirectToAction("OrderSummary", "Order");
                    if (roles.Contains(Roles.MagazzinoMaterie))
                        return RedirectToAction("RetireProduct", "Stock");
                    if (roles.Contains(Roles.Impiegato))
                        return RedirectToAction("AttendanceEmployee", "Attendance");
                    return RedirectToAction("Index", "Home");
                }
                    
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                _notyfToastService.Error("Email non valida.");
                return RedirectToAction("Login");
            }
 
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _notyfToastService.Error("Nessun account trovato con questa email.");
                return RedirectToAction("Login");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var returnUrl = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);
 
            bool emailResponse = _emailService.SendEmailForPasswordReset(returnUrl, email);

            if (emailResponse)
            {
                _notyfToastService.Success($"Mail inviata correttamente! (a { email })");
                return RedirectToAction("Login");
            }
            
            _notyfToastService.Error("Errore durante l'invio della mail di recupero password. Segnalare il problema.");
            return View("Login");
            
        }
        
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                Logger.Error($"Richiesta reset password (GET), nessun utente trovato con email: {email}");
                _notyfToastService.Error($"Nessun utente trovato con email: {email}");
                return RedirectToAction("Login");
            }
            
            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                Logger.Error($"Richiesta reset password (POST), nessun utente trovato con email: {model.Email}");
                _notyfToastService.Error($"Nessun utente trovato con email: {model.Email}");
                RedirectToAction("ResetPassword");
            }
            
            var resetResult = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if(!resetResult.Succeeded)
            {
                foreach (var error in resetResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View(model);
            }
            return RedirectToAction("ResetPasswordConfirmed");
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmed()
        {
            Logger.Info($"Password resettata per l'utente: {User.Identity.Name}.");
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> AddMember([FromBody] ProfileViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.NewUser.Email);
                if (user != null)
                    throw new Exception("Email già utilizzata.");
                return Json(new { Result = true, Data = _accountHelper.AddSubAccount(model)});
            }
            catch (Exception e)
            {
                return Json(new { Result = false, Message = e.Message });
            }
        }
        
        public JsonResult DeleteAccount(string accountId)
        {
            try
            {
                var deleteResult = _accountHelper.DeleteAccountByID(accountId);
                return new JsonResult(new { Result = deleteResult, ToastMessage = "Account eliminato correttamente." });
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error delete account: {accountId}");
                return new JsonResult(new { Error = e, ToastMessage = $"Errore durante l\\'eliminazione dell\\'account." });
            }
        }
    }
}
