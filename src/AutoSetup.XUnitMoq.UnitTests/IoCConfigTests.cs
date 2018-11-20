using Xunit;

namespace AutoSetup.XUnitMoq.UnitTests
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
