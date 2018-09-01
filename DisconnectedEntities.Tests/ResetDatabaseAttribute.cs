using System.Reflection;
using Xunit.Sdk;

namespace DisconnectedEntities.Tests
{
    public class ResetDatabaseAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            DbFixture.ResetDatabaseAsync().Wait();
        }
    }
}