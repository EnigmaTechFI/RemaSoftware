using Microsoft.AspNetCore.Identity;
using RemaSoftware.Domain.Constants;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Data
{
    public static class DbInitializer
    {
        public static void SeedUsersAndRoles(UserManager<MyUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext _context)
        {
            var op =_context.Operations.SingleOrDefault(s => s.Name == OtherConstants.COQ);
            if (op == null)
            {
                _context.Add(new Operation()
                {
                    Name = OtherConstants.COQ,
                    Description = OtherConstants.COQ
                });
            }
            
            op =_context.Operations.SingleOrDefault(s => s.Name == OtherConstants.EXTRA);
            if (op == null)
            {
                _context.Add(new Operation()
                {
                    Name = OtherConstants.EXTRA,
                    Description = OtherConstants.EXTRA
                });
            }
            _context.SaveChanges();
            string[] roleNames = { Roles.Admin, Roles.Dipendente, Roles.Cliente, Roles.Machine, Roles.COQ};
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
                var addedRole = userManager.AddToRolesAsync(adm1, new [] { Roles.Admin }).Result;
                
            }
            
            var adm2 = userManager.FindByEmailAsync("rema.pul@gmail.com").Result;
            if (adm2 == null)
            {
                adm2 = new MyUser
                {
                    UserName = "administrator",
                    Email = "rema.pul@gmail.com"
                };

                IdentityResult result = userManager.CreateAsync(adm2, "Remapul2023!").Result;
                
                var addedRole = userManager.AddToRolesAsync(adm2, new [] { Roles.Admin}).Result;
            }
            
            var dip1 = userManager.FindByEmailAsync("rema.magazzino@gmail.com").Result;
            if (dip1 == null)
            {
                dip1 = new MyUser
                {
                    UserName = "magazzino",
                    Email = "rema.magazzino@gmail.com"
                };

                IdentityResult result = userManager.CreateAsync(dip1, "RemaPul2022!").Result;
                
                var addedRole = userManager.AddToRolesAsync(dip1, new [] { Roles.Dipendente }).Result;
            }
            
            var dip2 = userManager.FindByEmailAsync("coq.rema.pul@gmail.com").Result;
            if (dip2 == null)
            {
                dip2 = new MyUser
                {
                    UserName = "Controllo",
                    Email = "coq.rema.pul@gmail.com"
                };

                IdentityResult result = userManager.CreateAsync(dip2, "RemaPul2022!").Result;
                
                var addedRole = userManager.AddToRolesAsync(dip2,new [] { Roles.COQ }).Result;
            }
            
            //TODO: CAMBIARE EMAIL ACCOUNT
        }
    }
}