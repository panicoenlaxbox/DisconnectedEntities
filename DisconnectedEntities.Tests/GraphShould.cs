using DisconnectedEntities.Tests.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Respawn;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DisconnectedEntities.Tests
{
    public class DbFixture
    {
        private static readonly Checkpoint Checkpoint = new Checkpoint();
        private const string ConnectionString = @"Server=(LocalDB)\MSSQLLocalDB;Database=Shop;Trusted_Connection=True;";

        public DbFixture()
        {
            using (var context = CreateDbContext())
            {
                context.Database.Migrate();
            }

            Checkpoint.TablesToIgnore = new[]
            {
                "__EFMigrationsHistory"
            };
        }

        public ShopContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(ConnectionString)
                .Options;
            return new ShopContext(options);
        }

        public static async Task ResetDatabaseAsync()
        {
            await Checkpoint.Reset(ConnectionString);
        }
    }

    [CollectionDefinition("DbCollection")]
    public class HostCollectionFixture : ICollectionFixture<DbFixture>
    {

    }

    public class ResetDatabaseAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            DbFixture.ResetDatabaseAsync().Wait();
        }
    }

    [Collection("DbCollection")]
    public class GraphShould
    {
        private readonly DbFixture _fixture;
        private readonly ITestOutputHelper _logger;

        public GraphShould(DbFixture fixture, ITestOutputHelper logger)
        {
            _fixture = fixture;
            _logger = logger;
        }

        [Fact]
        [ResetDatabase]
        public void add_graph_will_set_all_entities_in_added_entity_state()
        {
            var customer = new Customer();
            customer.Country = new Country() { Name = "Spain" };
            customer.Orders.Add(new Order() { Units = 1, Amount = 10m });
            customer.Orders.Add(new Order() { Units = 2, Amount = 20m });

            using (var context = _fixture.CreateDbContext())
            {
                context.Customers.Add(customer);

                context.Entry(customer).State.Should().Be(EntityState.Added);
                context.Entry(customer.Country).State.Should().Be(EntityState.Added);
                foreach (var order in customer.Orders)
                {
                    context.Entry(order).State.Should().Be(EntityState.Added);
                }
            }
        }
    }
}
