using Microsoft.EntityFrameworkCore;

namespace DisconnectedEntities.Tests.Models
{
    public class ShopContext : DbContext
    {
        public ShopContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Customer> Customers { get; set; }
    }
}