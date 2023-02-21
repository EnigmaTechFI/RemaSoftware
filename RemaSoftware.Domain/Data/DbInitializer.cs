using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Data
{
    public static class DbInitializer
    {
        public static void SeedUsersAndRoles(UserManager<MyUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Dipendente", "Cliente" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = roleManager.RoleExistsAsync(roleName).Result;
                if (!roleExist)
                    roleResult = roleManager.CreateAsync(new IdentityRole(roleName)).Result;
            }
            
            var adm1 = userManager.FindByEmailAsync("lorenzo.vettori11@gmail.com").Result;
            if (adm1 == null)
            {
                adm1 = new MyUser
                {
                    UserName = "lore_vetto11",
                    Email = "lorenzo.vettori11@gmail.com"
                };

                IdentityResult result = userManager.CreateAsync(adm1, "Antani123!").Result;
                var addedRole = userManager.AddToRolesAsync(adm1, new [] { Roles.Admin, Roles.Dipendente }).Result;
                
            }
            
            var adm2 = userManager.FindByEmailAsync("rema.pul@gmail.com").Result;
            if (adm2 == null)
            {
                adm2 = new MyUser
                {
                    UserName = "administrator",
                    Email = "rema.pul@gmail.com"
                };

                IdentityResult result = userManager.CreateAsync(adm2, "RemaSrls2021!").Result;
                
                var addedRole = userManager.AddToRolesAsync(adm2, new [] { Roles.Admin, Roles.Dipendente }).Result;
            }
            
            var dip1 = userManager.FindByEmailAsync("rema.pul.user@gmail.com").Result;
            if (dip1 == null)
            {
                dip1 = new MyUser
                {
                    UserName = "rema-user",
                    Email = "rema.pul.user@gmail.com"
                };

                IdentityResult result = userManager.CreateAsync(dip1, "RemaPul2022!").Result;
                
                var addedRole = userManager.AddToRolesAsync(dip1, new [] { Roles.Dipendente }).Result;
            }
        }
    }
}