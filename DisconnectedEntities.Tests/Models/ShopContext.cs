using Microsoft.EntityFrameworkCore;

namespace DisconnectedEntities.Tests.Models
{
    public class ShopContext : DbContext
    {
        public ShopContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property<int>(c => c.Id).ValueGeneratedNever();
            base.OnModelCreating(modelBuilder);
        }
    }
}