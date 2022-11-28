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
        public DbSet<Product> Products { get; set; }
        public DbSet<Ddt_In> Ddt_In { get; set; }
        public DbSet<Ddt_Out> Ddt_Out { get; set; }
        public DbSet<ProductOperation> ProductOperations { get; set; }
        public DbSet<OperationTimeline> OperationTimelines { get; set; }

        public DbSet<Operation> Operations { get; set; }

        public DbSet<Order_Operation> Order_Operations { get; set; }

        public DbSet<Warehouse_Stock> Warehouse_Stocks { get; set; }
        
        #region Required
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order_Operation>().HasKey(a => new { a.OrderID, a.OperationID });
            modelBuilder.Entity<ProductOperation>().HasKey(a => new { a.Ddt_In_ID, a.OperationID });
            modelBuilder.Entity<Ddt_In>().HasKey(a => new { a.Ddt_In_ID});
            modelBuilder.Entity<Ddt_Out>().HasKey(a => new { a.Ddt_Out_ID});
            base.OnModelCreating(modelBuilder);
        }
        #endregion

    }
}

