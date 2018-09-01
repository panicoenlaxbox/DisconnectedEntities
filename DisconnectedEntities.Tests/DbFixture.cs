using Microsoft.EntityFrameworkCore;
using Respawn;
using System.Threading.Tasks;

namespace DisconnectedEntities.Tests
{
    public class DbFixture : EfFixture
    {
        private static readonly Checkpoint Checkpoint = new Checkpoint();

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


        public static async Task ResetDatabaseAsync()
        {
            await Checkpoint.Reset(ConnectionString);
        }
    }
}