using RemaSoftware.Domain.Models;

namespace RemaSoftware.WebApp.Models.AccountingViewModel
{
    public class ProfileViewModel
    {
        public int ClientId { get; set; }
        public MyUser NewUser { get; set; }
        public string Role { get; set; }

    }
}
