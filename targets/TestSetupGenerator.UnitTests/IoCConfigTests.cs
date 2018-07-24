using TestSetupGenerator.XUnitMoq;
using Xunit;

namespace TestSetupGenerator.UnitTests
{
   public class IoCConfigTests
    {
        [Fact]
        public void Valid()
        {
            IoCConfig.Container.Verify();
        }
    }
}
