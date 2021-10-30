using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using RemaSoftware.ContextModels;
using Microsoft.EntityFrameworkCore;

namespace RemaSoftware.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<MyUser> MyUsers { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Operation> Operations { get; set; }

        public DbSet<Order_Operation> Order_Operations { get; set; }

        public DbSet<Warehouse_Stock> Warehouse_Stocks { get; set; }
        
        #region Required
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order_Operation>().HasKey(a => new { a.OrderID, a.OperationID });
            base.OnModelCreating(modelBuilder);
        }
        #endregion

    }
}

