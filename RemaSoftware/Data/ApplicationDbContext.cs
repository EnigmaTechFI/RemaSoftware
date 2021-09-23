using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemaSoftware.ContextModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace RemaSoftware.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<MyUser> MyUsers { get; set; }

        #region Required
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Seed(modelBuilder);     //Seed
            base.OnModelCreating(modelBuilder);

        }

        public void Seed(ModelBuilder modelBuilder)
        {
            var hasher = new PasswordHasher<MyUser>();
            var user = new List<MyUser>
            {
                new MyUser{UserName="lore_vetto11", NormalizedUserName="LORE_VETTO11", Email="lorenzo.vettori11@gmail.com", NormalizedEmail="LORENZO.VETTORI11@GMAIL.COM", PasswordHash = hasher.HashPassword(null, "Patomilan11!"), Name="Lorenzo", Surname="Vettori", Birthday=DateTime.Parse("30-09-1998"), LockoutEnabled = true, SecurityStamp = Guid.NewGuid().ToString("D")},

            };

            modelBuilder.Entity<MyUser>().HasData(user);

        }
        #endregion

    }
}

