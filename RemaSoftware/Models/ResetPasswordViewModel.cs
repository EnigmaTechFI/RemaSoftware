using System.ComponentModel.DataAnnotations;

namespace RemaSoftware.Models.LoginViewModel
{
    public class ResetPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "La password e la password di conferma non coincidono.")]
        public string ConfermaPassword { get; set; }
        
        public string Email { get; set; }
        public string Token { get; set; }
    }
}