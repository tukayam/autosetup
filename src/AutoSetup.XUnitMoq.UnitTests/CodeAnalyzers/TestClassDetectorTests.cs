using AutoSetup.XUnitMoq.CodeAnalyzers;
using Xunit;

namespace AutoSetup.XUnitMoq.UnitTests.CodeAnalyzers
{
    public class TestClassDetectorTests
    {
        [Theory]
        [InlineData("Tests", true)]
        [InlineData("BlaTests", true)]
        [InlineData("BlaTest", false)]
        [InlineData("Bla", false)]
        public void ReturnsTrueIfNameEndsWithTests(string className, bool expected)
        {
            var actual = TestClassDetector.IsTestClass(className);
            Assert.Equal(expected, actual);
        }
    }
}
