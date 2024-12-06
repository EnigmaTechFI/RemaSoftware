﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace RemaSoftware.WebApp.Models.LoginViewModel
{
    public class ForgotPasswordViewModel
    {
        [BindProperty]
        public Forgot ForgotModel {get; set; }

        public class Forgot
        {
            [Required(ErrorMessage = "Il campo è obbligatorio")]
            [Display(Name = "Username/Email")]
            [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Formato E-mail non valido")]
            public string Email { get; set; }
        }

    }
}
