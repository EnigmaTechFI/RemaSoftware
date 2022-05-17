using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.WebApp.Models.LoginViewModel
{
    public class LoginViewModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Il campo è obbligatorio")]
            [Display(Name = "Username/Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Il campo è obbligatorio")]
            [DataType(DataType.Password)]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password errata")]
            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }

    }
}
