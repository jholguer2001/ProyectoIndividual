using Microsoft.EntityFrameworkCore;
using POS.Entities;

namespace POS.DataAccess
{
    public class POSDbContext : DbContext
    {
        public POSDbContext(DbContextOptions<POSDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure unique constraint for product per order (REQUIREMENT 3)
            modelBuilder.Entity<SalesOrderDetail>()
                .HasIndex(sod => new { sod.SalesOrderId, sod.ProductId })
                .IsUnique();

            // Configure precision for decimal fields
            modelBuilder.Entity<Product>()
                .Property(p => p.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SalesOrder>()
                .Property(so => so.Subtotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SalesOrder>()
                .Property(so => so.TaxAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SalesOrder>()
                .Property(so => so.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SalesOrderDetail>()
                .Property(sod => sod.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SalesOrderDetail>()
                .Property(sod => sod.LineTotal)
                .HasPrecision(18, 2);
        }
    }
}
