using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;
using Xunit;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.CodeAnalyzers
{
    public class ClassUnderTestNameFinderTests
    {
        private ClassUnderTestNameFinder _target;
        public ClassUnderTestNameFinderTests()
        {
            _target = new ClassUnderTestNameFinder();
        }

        [Theory]
        [InlineData("SomeClassTests", "SomeClass")]
        [InlineData("SomeClassUnitTests", "SomeClass")]
        [InlineData("SomeClassUnitTests_SomeMethod", "SomeClass")]
        [InlineData("SomeClassTest", "SomeClass")]
        public void ReturnsClassUnderTestName(string testClassName, string expected)
        {
            var actual = _target.GetClassUnderTestName(testClassName);

            Assert.Equal(expected, actual);
        }
    }
}
