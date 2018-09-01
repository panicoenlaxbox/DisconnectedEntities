using Xunit;

namespace DisconnectedEntities.Tests
{
    [CollectionDefinition("DbCollection")]
    public class DbCollectionFixture : ICollectionFixture<DbFixture>
    {

    }
}