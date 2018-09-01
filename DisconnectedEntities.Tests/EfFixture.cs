using DisconnectedEntities.Tests.Models;
using Microsoft.EntityFrameworkCore;

namespace DisconnectedEntities.Tests
{
    public class EfFixture
    {
        protected const string ConnectionString = @"Server=(LocalDB)\MSSQLLocalDB;Database=Shop;Trusted_Connection=True;";

        public ShopContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(ConnectionString)
                .Options;
            return new ShopContext(options);
        }

    }
}