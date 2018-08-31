using DisconnectedEntities.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DisconnectedEntities.Tests
{
    internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ShopContext>
    {
        public ShopContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB;Database=Shop;Trusted_Connection=True;")
                .Options;
            return new ShopContext(options);
        }
    }
}