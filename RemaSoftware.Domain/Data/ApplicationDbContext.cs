using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<MyUser> MyUsers { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<UserClient> UserClients { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Ddt_In> Ddts_In { get; set; }
        public DbSet<Ddt_Out> Ddts_Out { get; set; }
        public DbSet<Ddt_Association> Ddt_Associations { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<SubBatch> SubBatches { get; set; }
        public DbSet<BatchOperation> BatchOperations { get; set; }
        public DbSet<OperationTimeline> OperationTimelines { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Order_Operation> Order_Operations { get; set; }
        public DbSet<Warehouse_Stock> Warehouse_Stocks { get; set; }
        
        #region Required
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order_Operation>().HasKey(a => new { a.OrderID, a.OperationID });
            modelBuilder.Entity<UserClient>().HasKey(a => new { a.MyUserID, a.ClientID });
            modelBuilder.Entity<Ddt_In>().HasKey(a => new { a.Ddt_In_ID});
            modelBuilder.Entity<Ddt_Out>().HasKey(a => new { a.Ddt_Out_ID});
            modelBuilder.Entity<Ddt_Association>().HasKey(a => new { a.Ddt_In_ID, a.Ddt_Out_ID});
            // modelBuilder.Entity<OperationTimeline>().HasKey(a => new { a.SubBatchID, a.BatchOperationID});
            modelBuilder.Entity<OperationTimeline>().HasOne(a => a.SubBatch).WithMany(a => a.OperationTimelines).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<OperationTimeline>().HasOne(a => a.BatchOperation).WithMany(a => a.OperationTimelines);
            base.OnModelCreating(modelBuilder);
        }
        #endregion

    }
}

