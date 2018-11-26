using Xunit;

namespace AutoSetup.UnitTests
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
