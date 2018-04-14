using System.Linq;
using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;
using TestSetupGenerator.CodeAnalysis.UnitTests.Helpers;
using Xunit;

namespace TestSetupGenerator.CodeAnalysis.UnitTests.CodeAnalyzers
{
    public class ConstructorParametersExtractorTests
    {
        private ConstructorParametersExtractor _target;
        public ConstructorParametersExtractorTests()
        {
            _target = new ConstructorParametersExtractor();
        }

        [Fact]
        public void ReturnsAllConstructorParameters()
        {
            var textFilePath = "files.Class_WithConstructorWithTwoParameters.txt";
            var classDeclarationSyntax = ClassDeclarationProvider.GetClassDeclaration(textFilePath, "TestClass");

            var actual = _target.GetParametersOfConstructor(classDeclarationSyntax).ToList();

            Assert.Equal(2, actual.Count);

            var firstParameter = actual.First();
            Assert.Equal("someType",firstParameter.Identifier.Text);
            Assert.Equal("ISomeType", firstParameter.Type.ToString());

            var secondParameter = actual.Skip(1).First();
            Assert.Equal("someOtherType", secondParameter.Identifier.Text);
            Assert.Equal("ISomeOtherType", secondParameter.Type.ToString());
        }
    }
}
