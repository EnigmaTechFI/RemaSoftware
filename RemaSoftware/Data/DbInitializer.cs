using Microsoft.AspNetCore.Identity;
using RemaSoftware.ContextModels;

namespace RemaSoftware.Data
{
    public static class DbInitializer
    {
        public static void SeedUsers(UserManager<MyUser> userManager)
        {
            if (userManager.FindByEmailAsync("lorenzo.vettori11@gmail.com").Result == null)
            {
                var user = new MyUser
                {
                    UserName = "lore_vetto11",
                    Email = "lorenzo.vettori11@gmail.com"
                };

                IdentityResult result = userManager.CreateAsync(user, "Antani123!").Result;
            }

            if (userManager.FindByEmailAsync("rema.pul@gmail.com").Result == null)
            {
                var user = new MyUser
                {
                    UserName = "administrator",
                    Email = "rema.pul@gmail.com"
                };

                IdentityResult result = userManager.CreateAsync(user, "RemaSrls2021!").Result;
            }
        }
    }
}