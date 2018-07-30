using TestSetupGenerator.XUnitMoq;
using Xunit;

namespace TestSetupGenerator.IntegrationTests
{
    public class IoCConfigTests
    {
        [Fact]
        public void Valid()
        {
            new IoCConfig().Container.Verify();
        }
    }
}
