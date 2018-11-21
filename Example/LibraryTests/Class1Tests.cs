using LibraryUnderTest;
using Moq;
using Xunit;

namespace LibraryTests
{
    public class Class1Tests
    {
        private readonly Mock<IDependency> _dependency;
        private readonly Class1 _target;

        public Class1Tests()
        {
            _dependency = new Mock<IDependency>();
            _target = new Class1(_dependency.Object);
        }

        [Fact]
        public void Test1()
        {

        }
    }
}
