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
        public DbSet<Product> Products { get; set; }
        public DbSet<Ddt_In> Ddts_In { get; set; }
        public DbSet<Ddt_Out> Ddts_Out { get; set; }
        public DbSet<Ddt_Association> Ddt_Associations { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<SubBatch> SubBatches { get; set; }
        public DbSet<BatchOperation> BatchOperations { get; set; }
        public DbSet<OperationTimeline> OperationTimelines { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Warehouse_Stock> Warehouse_Stocks { get; set; }
        public DbSet<Label> Label { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Ddt_Supplier> Ddt_Suppliers { get; set; }
        public DbSet<DDT_Supplier_Association> DdtSupplierAssociations { get; set; }

        #region Required
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserClient>().HasKey(a => new { a.MyUserID, a.ClientID });
            modelBuilder.Entity<Ddt_In>().HasKey(a => a.Ddt_In_ID);
            modelBuilder.Entity<Ddt_Out>().HasKey(a => a.Ddt_Out_ID);
            modelBuilder.Entity<Ddt_Supplier>().HasKey(a => a.Ddt_Supplier_ID);
            modelBuilder.Entity<Ddt_Association>().HasKey(s => s.ID );
            modelBuilder.Entity<Ddt_Association>().HasOne(a => a.Ddt_In).WithMany(a => a.Ddt_Associations).HasForeignKey(s => s.Ddt_In_ID).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Ddt_Association>().HasOne(a => a.Ddt_Out).WithMany(a => a.Ddt_Associations).HasForeignKey(s => s.Ddt_Out_ID).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<DDT_Supplier_Association>().HasKey(s => s.Id);
            modelBuilder.Entity<DDT_Supplier_Association>().HasOne(a => a.Ddt_In).WithMany(a => a.DdtSupplierAssociations).HasForeignKey(s => s.Ddt_In_ID).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<DDT_Supplier_Association>().HasOne(a => a.Ddt_Supplier).WithMany(a => a.DdtSupplierAssociations).HasForeignKey(s => s.Ddt_Supplier_ID).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<OperationTimeline>().HasOne(a => a.SubBatch).WithMany(a => a.OperationTimelines).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<OperationTimeline>().HasOne(a => a.BatchOperation).WithMany(a => a.OperationTimelines).OnDelete(DeleteBehavior.NoAction);
            base.OnModelCreating(modelBuilder);
        }
        #endregion

    }
}

