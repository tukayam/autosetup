using AutoSetup.XUnitMoq;
using Xunit;

namespace AutoSetup.IntegrationTests
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
