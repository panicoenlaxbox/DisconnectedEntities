using Xunit;

namespace DisconnectedEntities.Tests
{
    [CollectionDefinition("EfCollection")]
    public class EfCollectionFixture : ICollectionFixture<EfFixture>
    {

    }
}