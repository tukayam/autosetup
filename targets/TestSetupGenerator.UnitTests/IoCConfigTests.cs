using TestSetupGenerator.XUnitMoq;
using Xunit;

namespace TestSetupGenerator.UnitTests
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
