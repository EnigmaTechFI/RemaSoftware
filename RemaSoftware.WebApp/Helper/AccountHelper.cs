using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NLog;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Data;
using RemaSoftware.Domain.Models;
using RemaSoftware.UtilityServices.Interface;
using RemaSoftware.WebApp.Models.AccountingViewModel;
using RemaSoftware.WebApp.Models.EmployeeViewModel;

namespace RemaSoftware.WebApp.Helper
{
    public class AccountHelper
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<MyUser> _userManager;
        private readonly IEmailService _emailService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public AccountHelper(ApplicationDbContext dbContext, UserManager<MyUser> userManager, IEmailService emailService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailService = emailService;
        }

        public MyUser AddSubAccount(ProfileViewModel model)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    MyUser newUser = new MyUser();
                    var userClient = new UserClient();
                    userClient.ClientID = model.ClientId;
                    newUser.UserClient = userClient;
                    newUser.UserName = model.NewUser.Email;
                    newUser.Email = model.NewUser.Email;

                    var password = PasswordGenerator();

                    IdentityResult result = _userManager.CreateAsync(newUser, password).Result;

                    if (result.Succeeded)
                    {
                        _userManager.AddToRolesAsync(newUser, new[] { Roles.Cliente });

                        try
                        {
                            _emailService.SendEmailNewAccount(newUser.Email, password);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e.Message, e);
                        }

                        transaction.Commit();
                    }
                    else
                    {
                        throw new Exception("Errore, la mail è già presente");
                    }

                    return newUser;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
        
        public async Task<MyUser> AddEmployeeAccount(EmployeeViewModel model)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    MyUser newUser = new MyUser();
                    newUser.UserName = model.Employee.Mail;
                    newUser.Email = model.Employee.Mail;

                    var password = PasswordGenerator();

                    IdentityResult result = _userManager.CreateAsync(newUser, password).Result;

                    if (result.Succeeded)
                    { 
                        await _userManager.AddToRolesAsync(newUser, new[] { Roles.Impiegato });

                        try
                        {
                            _emailService.SendEmailNewAccount(newUser.Email, password);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e.Message, e);
                        }

                        transaction.Commit();
                    }
                    else
                    {
                        throw new Exception("Errore, la mail è già presente");
                    }

                    return newUser;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
        
        private string PasswordGenerator()
        {

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var minusc = "abcdefghijklmnopqrstuvwxyz";
            var num = "0123456789";
            var specialChar = "!$&()=?^*@#";
            var stringChars = new char[9];
            var random = new Random();

            for (int i = 0; i < 2; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            for (int i = 2; i < 5; i++)
            {
                stringChars[i] = minusc[random.Next(minusc.Length)];
            }
            for (int i = 5; i < stringChars.Length; i++)
            {
                stringChars[i] = num[random.Next(num.Length)];
            }
            stringChars[8] = specialChar[random.Next(num.Length)];

            return new String(stringChars);
        }
        
        public async Task DeleteAccountByID(string AccountID)
        {
            var user = _dbContext.Users.SingleOrDefault(i => i.Id == AccountID);
            _dbContext.Remove(user);
            _dbContext.SaveChanges();
        }
    }
}
