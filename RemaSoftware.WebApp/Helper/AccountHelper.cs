﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RemaSoftware.Domain.Services;
using RemaSoftware.Domain.Data;
using RemaSoftware.Domain.Models;
using RemaSoftware.Domain.Services.Impl;
using RemaSoftware.UtilityServices;
using RemaSoftware.WebApp.Models.AccountingViewModel;
using static RemaSoftware.WebApp.Models.AccountingViewModel.AccountingViewModel;

namespace RemaSoftware.WebApp.Helper
{
    public class AccountHelper
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<MyUser> _userManager;
        private readonly IEmailService _emailService;

        public AccountHelper(ApplicationDbContext dbContext, UserManager<MyUser> userManager, IEmailService emailService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailService = emailService;
        }

        public MyUser AddSubAccount(ProfileViewModel model)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                MyUser newUser = new MyUser();
                var userClient = new UserClient();
                userClient.ClientID = model.ClientId;
                newUser.UserClient = userClient;
                newUser.UserName = "Account-" + model.NewUser.Email;
                newUser.Email = model.NewUser.Email;

                var password = PasswordGenerator();

                IdentityResult result = _userManager.CreateAsync(newUser, password).Result;

                if (result.Succeeded)
                {
                    var role = model.Role;

                    _userManager.AddToRolesAsync(newUser, new[] { role });
                        
                    //bool emailResponse = _emailService.SendEmailNewClientAccount(newUser.Email, password);

                    /*if (!emailResponse)
                        {
                            throw new Exception("Errore, mail con le credenziali di accesso non inviata correttamente");
                        }*/
                    transaction.Commit();
                }else
                {
                    throw new Exception("Errore, la mail è già presente");
                }
                return newUser;
                    
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                transaction.Commit();
                throw ex;
            }
        }
        
        private string PasswordGenerator()
        {

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var minusc = "abcdefghijklmnopqrstuvwxyz";
            var num = "0123456789";
            var stringChars = new char[8];
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

            return new String(stringChars);
        }
        
        public bool DeleteAccountByID(string AccountID)
        {
            try
            {
                var user = _dbContext.Users.SingleOrDefault(i => i.Id == AccountID);
                _dbContext.Remove(user);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
}